using System.Collections;
using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public abstract class Formula
    {
        /// <summary>
        /// Gets the unique formula name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the minimum number of arguments this formula can use.
        /// </summary>
        public int MinArgsCount { get; set; }
        /// <summary>
        /// Gets the maximum number of arguments this formula can use.
        /// </summary>
        public int MaxArgsCount { get; set; }
        /// <summary>
        /// Gets the formula information.
        /// </summary>
        public abstract FormulaInfo Info { get; }

        public Formula(string name, int minArgsCount, int maxArgsCount)
        {
            Name = name;
            MinArgsCount = minArgsCount;
            MaxArgsCount = maxArgsCount;
        }

        /// <summary>
        /// Gets the evaluated result.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract object Evaluate(params object[] args);
    }
}
