using Microsoft.EntityFrameworkCore;

namespace SpecFlow.PoC.Extensions;

/// <summary>
/// Register Database
/// </summary>
public static class DatabaseExtension
{
    /// <summary>
    /// Connection string to put in settings / Vault
    /// </summary>
    public static string ConnectionString => "Data Source=SQLiteSample.db";

    /// <summary>
    /// Add SQLite component
    /// </summary>
    /// <param name="services"></param>
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        /*var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "SQLiteSample.db");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
           //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           options.UseSqlite("Data Source=SampleApi.db"));
*/
        services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.UseSqlite(ConnectionString);
                options.UseNpgsql(configuration.GetConnectionString("weather-db"));
            }
        );
    }
}
