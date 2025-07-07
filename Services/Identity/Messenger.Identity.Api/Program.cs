using Hangfire;
using Hangfire.Redis.StackExchange;
using Messenger.Common.Extensions;
using Messenger.Identity.Api.Errors;
using Messenger.Identity.Api.Options;
using Messenger.Identity.Api.Services;
using Messenger.Identity.Core;
using Messenger.Identity.Core.Options;
using Messenger.Identity.Core.Services;

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
