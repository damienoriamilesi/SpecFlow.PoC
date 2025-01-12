using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace SpecFlow.PoC.Meters;

public static class OpenTelemetryExtension
{
    public static void RegisterOpenTelemetry(this IServiceCollection services)
    {
        services.AddScoped<EntryMeter>();
        services.AddOpenTelemetry()
            .WithMetrics(cfg =>
            {
                cfg.AddMeter(EntryMeter.MeterName)
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter();
            })
            .WithTracing(cfg =>
            {
                cfg.AddAspNetCoreInstrumentation();
            });

    }
}