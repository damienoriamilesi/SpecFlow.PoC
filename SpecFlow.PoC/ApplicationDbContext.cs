using Microsoft.EntityFrameworkCore;
using SpecFlow.PoC.Features;

namespace SpecFlow.PoC;

/// <inheritdoc />
public class ApplicationDbContext : DbContext
{
    /// <inheritdoc />
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Employee> Employees { get; set; }
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
