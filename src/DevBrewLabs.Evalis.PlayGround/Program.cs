using System;

namespace DevBrewLabs.Evalis.Benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            int iterations = 500;
            int argumentCount = 10000;

            if (args.Length > 0 && int.TryParse(args[0], out int customIter))
            {
                iterations = customIter;
            }

            if (args.Length > 1 && int.TryParse(args[1], out int customArgs))
            {
                argumentCount = customArgs;
            }

            EngineBenchmarkRunner.RunBenchmarks(iterations, argumentCount);
        }
    }
}
