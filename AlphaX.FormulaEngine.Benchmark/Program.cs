namespace AlphaX.FormulaEngine.Benchmark
{
    public class Program
    {  
        static void Main(string[] args)
        {
            var engine = new AlphaXFormulaEngine();
            FormulaEngineBenchmark.RunBenchmarks(engine, 1000);
        }
    }
}
