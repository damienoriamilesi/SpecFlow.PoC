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
        File.ReadAllText("test.json");
    }
    [Benchmark()]
    public void CheckReadAllText2()
    {
        File.ReadAllLines("test.json");
    }
}