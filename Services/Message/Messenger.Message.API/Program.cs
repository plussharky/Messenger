using System.Diagnostics;
using MassTransit.Logging;
using Messenger.Common.Extensions;
using Messenger.Messages.Api.Mappings;
using Messenger.Messages.Application;
using Messenger.Messages.Domain;
using Messenger.Messages.Infrastructure;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MessageProfile));

builder.Services.AddOpenTelemetry()
  .ConfigureResource(r => r.AddService(
      serviceName: "Messenger.Messages",
      serviceVersion: "1.0.0"))
  .WithTracing(tracing =>
  {
      tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri(
              builder.Configuration["Jaeger:CollectorUrl"]
              ?? "http://localhost:4317");
            otlp.Protocol = OtlpExportProtocol.Grpc;
        });
  });

builder.Services.AddDomain()
    .AddApplication()
    .AddInfrastructure(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
        builder.Configuration.GetSection("RabbitMQ").Bind);

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

builder.Services.AddJwtAuthentication(builder.Configuration.GetSection("Jwt"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S1118", Justification = "Marker class for testing purposes")]
public sealed partial class Program
{
}
