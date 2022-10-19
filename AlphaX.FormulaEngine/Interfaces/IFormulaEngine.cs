using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public interface IFormulaEngine
    {
        /// <summary>
        /// Gets the engine settings.
        /// </summary>
        FormulaEngineSettings Settings { get; }
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
        /// <summary>
        /// Removes formula from the engine.
        /// </summary>
        /// <param name="formulaName"></param>
        void RemoveFormula(string formulaName);
        /// <summary>
        /// Removes formula from the engine.
        /// </summary>
        /// <param name="formula"></param>
        void RemoveFormula(Formula formula);
        /// <summary>
        /// Gets information related to registered formulas.
        /// </summary>
        /// <returns></returns>
        IEnumerable<FormulaInfo> GetFormulas();
    }
}
