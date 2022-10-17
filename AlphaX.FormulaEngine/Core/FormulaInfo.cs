using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public class FormulaInfo
    {
        /// <summary>
        /// Gets the formula description.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets the formula arguments.
        /// </summary>
        public FormulaArgument[] Arguments { get; }

        public FormulaInfo(string description, params FormulaArgument[] arguments)
        {
            Description = description;
            Arguments = arguments;
        }
    }
}
