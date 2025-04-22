using Microsoft.EntityFrameworkCore;
using SpecFlow.PoC.Features;
#pragma warning disable CS1591

namespace SpecFlow.PoC;

/// <inheritdoc />
public class ApplicationDbContext : DbContext
{
    /// <inheritdoc />
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name); //.HasColumnType("VARCHAR");
            entity.Property(e => e.BirthdayDate); //.HasColumnType("DOUBLE");
        });

        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date);
            entity.Property(e => e.TemperatureC);
            //entity.Property(e => e.TemperatureF);
            entity.HasOne(e => e.Employee);
        });
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
}

internal static class TestFixture
{
    public static IEnumerable<Employee> BuildEmployees()
    {
        yield return new Employee{ BirthdayDate = new DateTime(1999, 1, 1), Name = "Doe1" };
        yield return new Employee{ BirthdayDate = new DateTime(1995, 1, 1), Name = "Doe2" };
        yield return new Employee{ BirthdayDate = new DateTime(1998, 1, 1), Name = "Doe3" };
        yield return new Employee{ BirthdayDate = new DateTime(1999, 1, 1), Name = "Doe4" };
        yield return new Employee{ BirthdayDate = new DateTime(1997, 1, 1), Name = "Doe5" };
    }
}
