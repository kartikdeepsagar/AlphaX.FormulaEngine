using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AlphaX.FormulaEngine.Benchmark
{
    public static class FormulaEngineBenchmark
    {
        private static Stopwatch _expressionWatch;
        private static Stopwatch _benchmarkWatch;
        private static IFormulaEngine _formulaEngine;

        public static void RunBenchmarks()
        {
            _formulaEngine = new AlphaXFormulaEngine();
            _expressionWatch = new Stopwatch();
            _benchmarkWatch = new Stopwatch();

            Console.WriteLine("Hold on! Starting Benchmark...");
            _benchmarkWatch.Start();
            Console.WriteLine();

            Console.WriteLine("Cooking expressions...");
            var expressions = CookExpressions();           
            Console.WriteLine("Expressions cooked...");

            Console.WriteLine();
            for (int index = 1; index <= expressions.Count; index++)
            {
                Console.WriteLine($"----BENCHMARK {index}/{expressions.Count}----");
                RunExpressionBenchmark(expressions[index - 1]);
            }

            _benchmarkWatch.Stop();
            Console.WriteLine($"Yay! Benchmark completed in {_benchmarkWatch.Elapsed.TotalSeconds} seconds.");
            Console.ReadKey();
        }

        private static List<FormulaExpression> CookExpressions()
        {
            var rnd = new Random();
            var expressions = new List<FormulaExpression>();

            for (int test = 1; test <= 5; test++)
            {
                expressions.Add(CreateSumFormulaExpressionWithIntegers(5000));
            }

            for (int test = 1; test <= 5; test++)
            {
                expressions.Add(CreateSumFormulaExpressionWithDoubles(5000));
            }

            for (int test = 1; test <= 5; test++)
            {
                expressions.Add(CreateAverageFormulaExpressionWithIntegers(5000));
            }

            for (int test = 1; test <= 5; test++)
            {
                expressions.Add(CreateAverageFormulaExpressionWithDoubles(5000));
            }

            for (int test = 1; test <= 5; test++)
            {
                expressions.Add(CreateUpperFormulaExpression(50000));
            }

            for (int test = 1; test <= 5; test++)
            {
                expressions.Add(CreateLowerFormulaExpression(50000));
            }

            return expressions;
        }

        private static void RunExpressionBenchmark(FormulaExpression expression)
        {
            _expressionWatch.Reset();
            Console.WriteLine($"Expression : {expression.Value.Substring(0, 50)}...)");
            Console.WriteLine($"Expression Length : {expression.Value.Length}");
            Console.WriteLine($"Total Arguments in expression : {expression.ArgumentsCount}");
            if (expression.ExpectedResult is string)
                Console.WriteLine($"Expected result : {expression.ExpectedResult.ToString().Substring(0, 50)}...");
            else
                Console.WriteLine($"Expected result : {expression.ExpectedResult}");

            Console.WriteLine($"Evaluating expression...");
            _expressionWatch.Start();
            var result = _formulaEngine.Evaluate(expression.Value);
            _expressionWatch.Stop();

            if (expression.ExpectedResult is string)
                Console.WriteLine($"Evaluation result : {result.Value.ToString().Substring(0, 50)}...");
            else
                Console.WriteLine($"Evaluation result : {result.Value}");

            Console.WriteLine($"AlphaX Expression evaluated in {_expressionWatch.Elapsed.TotalSeconds} seconds.");
            Console.WriteLine();
        }

        private static FormulaExpression CreateSumFormulaExpressionWithIntegers(int argumentCount)
        {
            var expression = new FormulaExpression();
            Random random = new Random();
            var builder = new StringBuilder();
            builder.Append("SUM(");
            var values = Enumerable.Range(1, argumentCount).Select(x => random.Next(1, 10000)).ToList();
            builder.Append(string.Join(",", values));
            builder.Append(")");
            expression.Value = builder.ToString();
            expression.ExpectedResult = values.Sum();
            expression.ArgumentsCount = argumentCount;
            return expression;
        }

        private static FormulaExpression CreateSumFormulaExpressionWithDoubles(int argumentCount)
        {
            var expression = new FormulaExpression();
            Random random = new Random();
            var builder = new StringBuilder();
            builder.Append("SUM(");
            var values = Enumerable.Range(1, argumentCount).Select(x => random.Next(1, 10000) * random.NextDouble()).ToList();
            builder.Append(string.Join(",", values));
            builder.Append(")");
            expression.Value = builder.ToString();
            expression.ExpectedResult = values.Sum();
            expression.ArgumentsCount = argumentCount;
            return expression;
        }

        private static FormulaExpression CreateAverageFormulaExpressionWithIntegers(int argumentCount)
        {
            var expression = new FormulaExpression();
            Random random = new Random();
            var builder = new StringBuilder();
            builder.Append("SUM(");
            var values = Enumerable.Range(1, argumentCount).Select(x => random.Next(1, 10000)).ToList();
            builder.Append(string.Join(",", values));
            builder.Append(")");
            expression.Value = builder.ToString();
            expression.ExpectedResult = values.Average();
            expression.ArgumentsCount = argumentCount;
            return expression;
        }

        private static FormulaExpression CreateAverageFormulaExpressionWithDoubles(int argumentCount)
        {
            var expression = new FormulaExpression();
            Random random = new Random();
            var builder = new StringBuilder();
            builder.Append("SUM(");
            var values = Enumerable.Range(1, argumentCount).Select(x => random.Next(1, 10000) * random.NextDouble()).ToList();
            builder.Append(string.Join(",", values));
            builder.Append(")");
            expression.Value = builder.ToString();
            expression.ExpectedResult = values.Average();
            expression.ArgumentsCount = argumentCount;
            return expression;
        }

        private static FormulaExpression CreateUpperFormulaExpression(int length)
        {
            var expression = new FormulaExpression();
            Random random = new Random();
            var builder = new StringBuilder();
            builder.Append("UPPER(");
            var value = RandomString(length, random, true);
            builder.Append($"\"{value}\"");
            builder.Append(")");
            expression.Value = builder.ToString();
            expression.ExpectedResult = value.ToUpper();
            expression.ArgumentsCount = 1;
            return expression;
        }

        private static FormulaExpression CreateLowerFormulaExpression(int length)
        {
            var expression = new FormulaExpression();
            Random random = new Random();
            var builder = new StringBuilder();
            builder.Append("LOWER(");
            var value = RandomString(length, random);
            builder.Append($"\"{value}\"");
            builder.Append(")");
            expression.Value = builder.ToString();
            expression.ExpectedResult = value.ToUpper();
            expression.ArgumentsCount = 1;
            return expression;
        }

        private static string RandomString(int size, Random random, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        public class FormulaExpression
        {
            public string Value { get; set; }
            public int ArgumentsCount { get; set; }
            public object ExpectedResult { get; set; }
        }
    }
}
