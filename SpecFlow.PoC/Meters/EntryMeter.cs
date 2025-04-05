using System.Diagnostics.Metrics;

namespace SpecFlow.PoC.Meters;

/// <summary>
/// 
/// </summary>
public class EntryMeter
{
    public Meter Meter { get; private set; }
    public Counter<int> ReadsCounter { get; private set; }
    public static readonly string MeterName = "WeatherForecast.Meters";

    public EntryMeter()
    {
        Meter = new Meter(MeterName, "1.0.0");
        ReadsCounter = Meter.CreateCounter<int>("enter.reads",
            description: "Counts the number of reads of any entry.");
    }
}