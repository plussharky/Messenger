using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Messenger.Common.Extensions;

public static class OpenTelemetryServiceCollectionExtensions
{
    public static IServiceCollection AddMessengerOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string serviceVersion = "1.0.0",
        Action<TracerProviderBuilder>? configureTracing = null)
    {
        var jaegerUrl = configuration["Jaeger:CollectorUrl"] ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(
                serviceName: serviceName,
                serviceVersion: serviceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("MassTransit")
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri(jaegerUrl);
                        otlp.Protocol = OtlpExportProtocol.Grpc;
                    });

                configureTracing?.Invoke(tracing);
            });

        return services;
    }
}
