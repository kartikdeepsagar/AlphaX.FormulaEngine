using AlphaX.Parserz;
using AlphaX.Parserz.Extensions;
using AlphaX.Parserz.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AlphaX.FormulaEngine.Benchmark
{
    public class Program
    {  
        static void Main(string[] args)
        {
            FormulaEngineBenchmark.RunBenchmarks(500);
        }
    }
}
