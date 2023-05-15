using AlphaX.FormulaEngine.Formulas;

namespace AlphaX.FormulaEngine
{
    internal static class DefaultFormulas
    {
        public static void Load(AlphaXFormulaEngine engine)
        {
            engine.AddFormula(new SumFormula());
            engine.AddFormula(new AverageFormula());
            engine.AddFormula(new LowerFormula());
            engine.AddFormula(new UpperFormula());
            engine.AddFormula(new TextSplitFormula());
            engine.AddFormula(new TodayFormula());
            engine.AddFormula(new ConcatFormula());
            engine.AddFormula(new NowFormula());
            engine.AddFormula(new IFFormula());
        }
    }
}
