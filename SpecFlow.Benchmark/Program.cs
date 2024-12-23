// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var result = BenchmarkRunner.Run<BenchmarkToExecute>();

[SimpleJob] // Mean / Error / StdDev
[MemoryDiagnoser]
public class BenchmarkToExecute
{
    private string fileContent;

    [Benchmark()]
    public void CheckReadAllText()
    {
        var test = Task.Run(() => "toto").Result;
    }
    [Benchmark()]
    public void CheckReadAllText2()
    {
        var test = "toto";
    }
}