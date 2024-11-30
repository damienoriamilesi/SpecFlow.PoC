using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace SpecFlow.Tests;

[MemoryDiagnoser()]
public class UnitTest1
{
    [Fact]
    [Benchmark]
    public void Test1()
    {
        var text = "TODO: " + Guid.NewGuid();

        var result = text.ToList();
    }
}