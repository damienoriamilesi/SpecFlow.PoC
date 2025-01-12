using Microsoft.EntityFrameworkCore;

namespace SpecFlow.PoC.Extensions;

/// <summary>
/// Register Database 
/// </summary>
public static class DatabaseExtension
{
    /// <summary>
    /// Add SQLite component
    /// </summary>
    /// <param name="services"></param>
    public static void AddDatabase(this IServiceCollection services)
    {
        services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            options.UseSqlite("Filename=SQLiteSample.db")
        );
    }
}