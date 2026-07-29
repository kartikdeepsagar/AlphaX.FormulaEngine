using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevBrewLabs.Evalis;

namespace DevBrewLabs.Evalis.Benchmark
{
    public static class EngineBenchmarkRunner
    {
        private static readonly Random _random = new Random(42);

        public class BenchmarkCase
        {
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Expression { get; set; } = string.Empty;
            public object? ExpectedResult { get; set; }
        }

        public class BenchmarkResult
        {
            public string CaseName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public int Iterations { get; set; }
            public double ParseMs { get; set; }
            public double ParseUsPerOp { get; set; }
            public double WarmEvalMs { get; set; }
            public double WarmEvalMsPerOp { get; set; }
            public double WarmEvalUsPerOp { get; set; }
            public double FullEvalMs { get; set; }
            public double FullEvalUsPerOp { get; set; }
            public double OpsPerSecond { get; set; }
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }

        public static void RunBenchmarks(int iterations = 1000, int argCount = 10000)
        {
            var engine = new FormulaEngine();
            var benchmarkCases = BuildBenchmarkCases(argCount);

            Console.WriteLine(new string('=', 90));
            Console.WriteLine($"                DEVBREWLABS.EVALIS BENCHMARK SUITE");
            Console.WriteLine($"  Config: {iterations:N0} iter/case | {benchmarkCases.Count} Cases | {argCount:N0} Arg Array Size");
            Console.WriteLine(new string('=', 90) + "\n");

            var results = new List<BenchmarkResult>();

            PrintTableHeader();

            foreach (var testCase in benchmarkCases)
            {
                var result = RunSingleBenchmark(engine, testCase, iterations);
                results.Add(result);
                PrintBenchmarkRow(result);
            }

            PrintSummary(results, iterations);
        }

        private static List<BenchmarkCase> BuildBenchmarkCases(int count)
        {
            var cases = new List<BenchmarkCase>();

            // Case 1: Large Integer SUM Aggregation (10,000 args)
            var intValues = Enumerable.Range(1, count).Select(_ => _random.Next(1, 100)).ToList();
            string intListStr = string.Join(",", intValues);
            cases.Add(new BenchmarkCase
            {
                Name = $"Large Integer SUM ({count:N0} args)",
                Category = "Aggregation",
                Expression = $"SUM({intListStr})",
                ExpectedResult = (double)intValues.Sum()
            });

            // Case 2: Complex Nested Arithmetic & Math Functions
            cases.Add(new BenchmarkCase
            {
                Name = "Complex Nested Math & Rounding",
                Category = "Arithmetic",
                Expression = "((100.5 * 2.5 + CEILING(4.2) - FLOOR(9.8)) * ROUND(123.456, 2) / ABS(-50.5)) + (SUM(10, 20, 30, 40) - AVERAGE(5, 15, 25))"
            });

            // Case 3: Trigonometric & Exponential Formulas
            cases.Add(new BenchmarkCase
            {
                Name = "Trig & Exponential Formulas",
                Category = "Arithmetic",
                Expression = "SQRT(POWER(3, 2) + POWER(4, 2)) + LOG10(1000) * EXP(1.5) - PI()"
            });

            // Case 4: Complex Nested String Transformations
            cases.Add(new BenchmarkCase
            {
                Name = "Nested String Transformations",
                Category = "String",
                Expression = "CONCAT(UPPER(\"evalis_engine\"), \"_\", LOWER(\"BENCHMARK_TEST\"), \"_\", REPLACE(\"Version 1.0\", \"1.0\", \"2.0\"))",
                ExpectedResult = "EVALIS_ENGINE_benchmark_test_Version 2.0"
            });

            // Case 5: Multi-Condition Logical & IF Branching
            cases.Add(new BenchmarkCase
            {
                Name = "Multi-Condition IF Branching",
                Category = "Logical",
                Expression = "IF(AND(10 > 5, OR(UPPER(\"Evalis\") = \"EVALIS\", 1 = 2)), SUM(100, 200, 300), AVERAGE(10, 20, 30))",
                ExpectedResult = 600.0
            });

            // Case 6: Multi-Branch IFS Decision Matrix
            cases.Add(new BenchmarkCase
            {
                Name = "Multi-Branch IFS Matrix",
                Category = "Logical",
                Expression = "IFS(10 > 20, \"Low\", 50 = 50, \"Medium\", 100 < 200, \"High\", true, \"Default\")",
                ExpectedResult = "Medium"
            });

            // Case 7: Array Operations & Membership Search
            cases.Add(new BenchmarkCase
            {
                Name = "Array Membership Search",
                Category = "Array",
                Expression = "ARRAYCONTAINS(ARRAY(10, 20, 30, 40, 50, 60, 70, 80, 90, 100), 50)",
                ExpectedResult = true
            });

            // Case 8: Deeply Nested Function Call Tree (15 Levels Deep)
            cases.Add(new BenchmarkCase
            {
                Name = "Deep Call Tree (15 Levels)",
                Category = "Recursion",
                Expression = "SUM(1, SUM(2, SUM(3, SUM(4, SUM(5, SUM(6, SUM(7, SUM(8, SUM(9, SUM(10, SUM(11, SUM(12, SUM(13, SUM(14, 15)))))))))))))))",
                ExpectedResult = 120.0
            });

            // Case 9: Real-World Business Discount Rule Calculation
            cases.Add(new BenchmarkCase
            {
                Name = "Real-World Business Rule",
                Category = "Business",
                Expression = "IF(SUM(150, 250, 350, 450) >= 1000 && CONTAINS(UPPER(\"Enterprise Subscription Plan\"), \"ENTERPRISE\"), ROUND(SUM(150, 250, 350, 450) * 0.85, 2), SUM(150, 250, 350, 450))",
                ExpectedResult = 1020.0
            });

            // Case 10: Error Handling & Coalesce Evaluation
            cases.Add(new BenchmarkCase
            {
                Name = "Error Handling & Coalesce",
                Category = "Logical",
                Expression = "IFERROR(100 / 0, COALESCE(null, \"\", \"Fallback Value\"))",
                ExpectedResult = "Fallback Value"
            });

            return cases;
        }

        private static BenchmarkResult RunSingleBenchmark(FormulaEngine engine, BenchmarkCase testCase, int iterations)
        {
            var result = new BenchmarkResult
            {
                CaseName = testCase.Name,
                Category = testCase.Category,
                Iterations = iterations
            };

            try
            {
                // 1. Measure Parse-Only (Cold Parse)
                var swParse = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    _ = engine.Parse(testCase.Expression);
                }
                swParse.Stop();
                result.ParseMs = swParse.ElapsedTotalMilliseconds();
                result.ParseUsPerOp = (result.ParseMs * 1000.0) / iterations;

                // 2. Measure Warm Evaluation (Cached AST in engine)
                engine.Evaluate(testCase.Expression);

                var swWarm = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    _ = engine.Evaluate(testCase.Expression);
                }
                swWarm.Stop();
                result.WarmEvalMs = swWarm.ElapsedTotalMilliseconds();
                result.WarmEvalMsPerOp = result.WarmEvalMs / iterations;
                result.WarmEvalUsPerOp = (result.WarmEvalMs * 1000.0) / iterations;
                result.OpsPerSecond = result.WarmEvalMs > 0 ? (iterations / (result.WarmEvalMs / 1000.0)) : 0;

                // 3. Full Eval = Parse + Warm Eval per op
                result.FullEvalMs = result.ParseMs + result.WarmEvalMs;
                result.FullEvalUsPerOp = result.ParseUsPerOp + result.WarmEvalUsPerOp;

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static double ElapsedTotalMilliseconds(this Stopwatch sw)
        {
            return (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000.0;
        }

        private static void PrintTableHeader()
        {
            Console.WriteLine($"{"Test Case",-28} | {"Category",-11} | {"Total",9} | {"ms/op",9} | {"µs/op",8} | {"ops/sec",12}");
            Console.WriteLine(new string('-', 88));
        }

        private static void PrintBenchmarkRow(BenchmarkResult res)
        {
            if (!res.IsSuccess)
            {
                Console.WriteLine($"{TruncateString(res.CaseName, 26),-28} | {res.Category,-11} | {"ERROR",9} | {"ERROR",9} | {"ERROR",8} | {res.ErrorMessage,12}");
                return;
            }

            string totalMsStr = $"{res.WarmEvalMs,6:N1}ms";
            string avgMsStr = $"{res.WarmEvalMsPerOp,7:F4}ms";
            string avgUsStr = $"{res.WarmEvalUsPerOp,6:N1}µs";
            string opsStr = $"{res.OpsPerSecond,10:N0}/s";

            Console.WriteLine($"{TruncateString(res.CaseName, 26),-28} | {res.Category,-11} | {totalMsStr,9} | {avgMsStr,9} | {avgUsStr,8} | {opsStr,12}");
        }

        private static void PrintSummary(List<BenchmarkResult> results, int iterations)
        {
            var successful = results.Where(r => r.IsSuccess).ToList();
            if (successful.Count == 0) return;

            double sumTotalMs = successful.Sum(r => r.WarmEvalMs);
            double avgMsPerOp = successful.Average(r => r.WarmEvalMsPerOp);
            double avgUsPerOp = successful.Average(r => r.WarmEvalUsPerOp);
            double totalOps = (double)successful.Count * iterations;
            double overallOpsSec = sumTotalMs > 0 ? (totalOps / (sumTotalMs / 1000.0)) : 0;

            string totalMsStr = $"{sumTotalMs,6:N1}ms";
            string avgMsStr = $"{avgMsPerOp,7:F4}ms";
            string avgUsStr = $"{avgUsPerOp,6:N1}µs";
            string opsStr = $"{overallOpsSec,10:N0}/s";

            Console.WriteLine(new string('-', 88));
            Console.WriteLine($"{"TOTAL / OVERALL METRICS",-28} | {"All",-11} | {totalMsStr,9} | {avgMsStr,9} | {avgUsStr,8} | {opsStr,12}");
            Console.WriteLine(new string('=', 88) + "\n");
        }

        private static string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return str.Length <= maxLength ? str : str.Substring(0, maxLength - 3) + "...";
        }
    }
}
