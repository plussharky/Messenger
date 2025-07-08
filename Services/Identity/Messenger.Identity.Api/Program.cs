using System.Diagnostics;
using Hangfire;
using Hangfire.Redis.StackExchange;
using MassTransit.Logging;
using Messenger.Common.Extensions;
using Messenger.Identity.Api.Errors;
using Messenger.Identity.Api.Options;
using Messenger.Identity.Api.Services;
using Messenger.Identity.Core;
using Messenger.Identity.Core.Options;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7103", "http://localhost:5103")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddOpenTelemetry()
  .ConfigureResource(r => r.AddService(
      serviceName: "Messenger.Identity",
      serviceVersion: "1.0.0"))
  .WithTracing(tracing =>
  {
      tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRedisInstrumentation()
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddNpgsql()
        .AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri(
                builder.Configuration["Jaeger:CollectorUrl"]
                ?? "http://localhost:4317");
            otlp.Protocol = OtlpExportProtocol.Grpc;
        });
  });

var connectionStringValue = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Connection string 'Redis' not found.");

builder.Services.AddSingleton(new ConnectionString
{
    Value = connectionStringValue,
});
builder.Services.AddSingleton(new RedisConnectionString
{
    Value = redisConnectionString,
});

builder.Services.AddIdentityCoreServices(
    builder.Configuration.GetSection("Jwt").Bind,
    builder.Configuration.GetSection("RabbitMQ").Bind);
builder.Services.AddAutoMapper(typeof(Messenger.Identity.Api.Mapping.LoginProfile).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IErrorHandler, ErrorHandler>();

builder.Services.AddHangfire((serviceProvider, configuration) => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseRedisStorage(serviceProvider.GetRequiredService<RedisConnectionString>().Value));
builder.Services.AddHangfireServer();
builder.Services.AddHostedService<RecurringJobsHostedService>();

builder.Services.AddJwtAuthentication(builder.Configuration.GetSection("Jwt"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire");

await app.RunAsync();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S1118", Justification = "Marker class for testing purposes")]
public sealed partial class Program
{
}
