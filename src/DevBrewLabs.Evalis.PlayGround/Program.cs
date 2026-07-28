namespace DevBrewLabs.Evalis.PlayGround
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FormulaEngine engine = new FormulaEngine();
            FormulaEngineBenchmark.RunBenchmarks(engine, 1000);
        }
    }
}
