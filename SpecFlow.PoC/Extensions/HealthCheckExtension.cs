using System.Text;
using System.Text.Json;
using HealthChecks.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SpecFlow.PoC.Features;

namespace SpecFlow.PoC.Extensions;

/// <summary>
/// Healthcheck methods
/// </summary>
public static class HealthCheckExtension
{
    /// <summary>
    /// Registering Healthchecks
    /// </summary>
    /// <param name="services"></param>
    public static void AddHealthchecks(this IServiceCollection services)
    {
        var connectionString = "Data Source=SQLiteSample.db";
        services.AddHealthChecks()
            .AddCheck("SQLite Db", new SqliteHealthCheck(connectionString, $"SELECT 1 FROM {nameof(Employee)}s"));
    }

    /// <summary>
    /// Get Healthcheck custom status
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="healthReport"></param>
    /// <returns></returns>
    public static Task WriteResponse(HttpContext httpContext, HealthReport healthReport)
    {
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value.GetType());
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return httpContext.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}