#pragma warning disable CS1591
namespace SpecFlow.PoC.Features;

public class WeatherForecast
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>2024-01-01</example>
    public DateTime Date { get; set; }

    /// <summary>
    /// Temperature in Celsius
    /// </summary>
    /// <example>42</example>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Temperature in Farenheit
    /// </summary>
    /// <example>666</example>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Summary of the forecast
    /// </summary>
    /// <example>fjmnzeamjvknaerjmvnaerjn</example>
    public string? Summary { get; set; }
}

