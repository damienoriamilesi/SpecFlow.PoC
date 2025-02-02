// See https://aka.ms/new-console-template for more information

using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotMemory;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<BenchmarkToExecute>();


//[SimpleJob] // Mean / Error / StdDev
//[MemoryDiagnoser]

// Enables dotMemory profiling for all jobs
[DotMemoryDiagnoser]
// Adds the default "external-process" job
// Profiling is performed using dotMemory Command-Line Profiler
// See: https://www.jetbrains.com/help/dotmemory/Working_with_dotMemory_Command-Line_Profiler.html
[SimpleJob]
// Adds an "in-process" job
// Profiling is performed using dotMemory SelfApi
// NuGet reference: https://www.nuget.org/packages/JetBrains.Profiler.SelfApi
[InProcess]
public class BenchmarkToExecute
{
    //private string? fileContent;

    [Benchmark()]
    public string CheckReadAllText()
    {
        Lazy<string> x = new Lazy<string>(() => new StringBuilder().ToString());
        var s = x.Value;
        return s;
    }
    [Benchmark()]
    public string CheckReadAllText2()
    {
        var x = new StringBuilder().ToString();
        var s = x;
        return s;
    }
}