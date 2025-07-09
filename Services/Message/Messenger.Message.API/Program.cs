using System.Diagnostics;
using Messenger.Common.Extensions;
using Messenger.Messages.Api.Mappings;
using Messenger.Messages.Application;
using Messenger.Messages.Domain;
using Messenger.Messages.Infrastructure;
using OpenTelemetry.Trace;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MessageProfile));

builder.Services.AddMessengerOpenTelemetry(
    builder.Configuration,
    "Messenger.Messages",
    configureTracing: tracing =>
    {
        tracing.AddEntityFrameworkCoreInstrumentation();
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
