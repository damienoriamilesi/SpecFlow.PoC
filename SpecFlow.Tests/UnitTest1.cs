using System.ComponentModel;
using Testcontainers.PostgreSql;

namespace SpecFlow.Tests;

public class DummyContainerTest : ContainerTestBase
{
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
public abstract class ContainerTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithReuse(false)
        .Build();

    public async Task InitializeAsync() => await _container.StartAsync();

    public async Task DisposeAsync() => await _container.DisposeAsync();
}