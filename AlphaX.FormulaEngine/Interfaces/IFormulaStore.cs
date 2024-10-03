using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public interface IFormulaStore
    {
        /// <summary>
        /// Adds a new formula.
        /// </summary>
        /// <param name="formula"></param>
        void Add(Formula formula);
        /// <summary>
        /// Gets if the formula is present in store.
        /// </summary>
        /// <param name="formulaName"></param>
        /// <returns></returns>
        bool Contains(string formulaName);
        /// <summary>
        /// Gets a formula by name.
        /// </summary>
        /// <param name="formulaName"></param>
        /// <returns></returns>
        Formula Get(string formulaName);
        /// <summary>
        /// Gets information of all the formulas available.
        /// </summary>
        /// <returns></returns>
        IEnumerable<FormulaInfo> GetAll();
        /// <summary>
        /// Removes a formula.
        /// </summary>
        /// <param name="formulaName"></param>
        void Remove(string formulaName);
    }
}
