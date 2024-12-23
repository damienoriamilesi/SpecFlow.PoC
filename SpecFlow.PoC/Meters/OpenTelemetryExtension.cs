using OpenTelemetry.Metrics;

namespace SpecFlow.PoC.Meters;

public static class OpenTelemetryExtension
{
    public static void RegisterOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(cfg =>
            {
                cfg.AddMeter(EntryMeter.MeterName)
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter();
            });

    }
}