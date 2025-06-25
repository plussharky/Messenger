using Messenger.Common.Extensions;
using Messenger.Messages.Api.Mappings;
using Messenger.Messages.Application;
using Messenger.Messages.Domain;
using Messenger.Messages.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MessageProfile));

builder.Services.AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7103")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowSpecificOrigin");

await app.RunAsync();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S1118", Justification = "Marker class for testing purposes")]
public sealed partial class Program
{
}
