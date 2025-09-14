using DotNet.Testcontainers.Builders;

using Microsoft.EntityFrameworkCore;

using SpecFlow.PoC;
using SpecFlow.PoC.Features;

using Testcontainers.PostgreSql;

namespace SpecFlow.Tests;

[Collection(Consts.IntegrationTest)]
public class DummyContainerTest
{
    private readonly DbContainerFixture _dbContainerFixture;

    public DummyContainerTest(DbContainerFixture dbContainerFixture)
    {
        _dbContainerFixture = dbContainerFixture;
    }

    [Fact]
    public void Test1()
    {
        int[] numbers = { 1,2,3,4,5,6,7,8,9,10};

        var resultSequence = numbers[4..]; // Sequence
        Assert.Contains(5, resultSequence);
        Assert.Contains(6, resultSequence);
        Assert.Contains(7, resultSequence);
        Assert.Contains(8, resultSequence);
        var resultIndexed = numbers[^2];
        Assert.Equal(9, resultIndexed);
    }
}

[Collection(Consts.IntegrationTest)]
public class AnotherDummyContainerTest
{
    private readonly DbContainerFixture _dbContainerFixture;

    public AnotherDummyContainerTest(DbContainerFixture dbContainerFixture)
    {
        _dbContainerFixture = dbContainerFixture;
    }

    [Fact]
    public void DbContainer_Should_BeInitializedInCollectionFixture()
    {
        // Ensure database schema is created
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_dbContainerFixture.ConnectionString)
            .Options;
        var dbContext = new ApplicationDbContext(options);
        dbContext.Employees.Add(new Employee { Name = "EmployeeFromTest", BirthdayDate = DateTime.UtcNow });
        dbContext.SaveChanges();

        Assert.Single(dbContext.Employees);
    }

    [Fact]
    public void Basic_Sequence_Test()
    {
        int[] numbers = { 1,2,3,4,5,6,7,8,9,10};

        var resultSequence = numbers[4..]; // Sequence
        Assert.Contains(5, resultSequence);
        Assert.Contains(6, resultSequence);
        Assert.Contains(7, resultSequence);
        Assert.Contains(8, resultSequence);
        var resultIndexed = numbers[^2];
        Assert.Equal(9, resultIndexed);
    }
}

[Collection(Consts.IntegrationTest)]
public class AnotherDummyContainerTest2
{
    private readonly DbContainerFixture _dbContainerFixture;

    public AnotherDummyContainerTest2(DbContainerFixture dbContainerFixture)
    {
        _dbContainerFixture = dbContainerFixture;
    }

    [Fact]
    public void Test1()
    {
        int[] numbers = { 1,2,3,4,5,6,7,8,9,10};

        var resultSequence = numbers[4..]; // Sequence
        Assert.Contains(5, resultSequence);
        Assert.Contains(6, resultSequence);
        Assert.Contains(7, resultSequence);
        Assert.Contains(8, resultSequence);
        var resultIndexed = numbers[^2];
        Assert.Equal(9, resultIndexed);
    }
}

/// <summary>
/// Init TestContainer with Postgre container
/// </summary>
public class DbContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        //.WithReuse(false)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    public ApplicationDbContext ContainerDbContext { get; set; }

    public async Task InitializeAsync()
    {
        // await _container.StartAsync();
        // var cnxString = _container.GetConnectionString();
        // //Host=127.0.0.1;Port=54566;Database=postgres;Username=postgres;Password=postgres
        // ConnectionString = cnxString;

        await _container.StartAsync();

        // Ensure database schema is created
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
    }

    public string ConnectionString => _container.GetConnectionString();

    public async Task DisposeAsync() => await _container.StopAsync();
}

// Define a test collection that shares the fixture
[CollectionDefinition(Consts.IntegrationTest)]
public class PostgresCollection : ICollectionFixture<DbContainerFixture> { }

public static class Consts
{
    public const string IntegrationTest = "IntegrationTest";
}
