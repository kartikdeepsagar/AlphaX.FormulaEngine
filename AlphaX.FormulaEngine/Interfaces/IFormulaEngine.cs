using AlphaX.FormulaEngine.Formulas;

namespace AlphaX.FormulaEngine
{
    public interface IFormulaEngine
    {
        /// <summary>
        /// Gets the evaluated result of the provided formula expression.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEvaluationResult Evaluate(string input);
        /// <summary>
        /// Adds a new formula to the engine.
        /// </summary>
        /// <param name="formula"></param>
        void AddFormula(Formula formula);
    }
}
