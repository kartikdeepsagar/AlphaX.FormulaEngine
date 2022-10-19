using AlphaX.Parserz;
using AlphaX.Parserz.Extensions;
using AlphaX.Parserz.Results;
using System.Globalization;

namespace AlphaX.FormulaEngine.Benchmark
{
    public class Program
    {  
        static void Main(string[] args)
        {
            FormulaEngineBenchmark.RunBenchmarks(5000);
        }
    }
}
