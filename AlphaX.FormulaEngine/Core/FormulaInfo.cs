﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaX.FormulaEngine
{
    public class FormulaInfo
    {
        private List<FormulaArgument> _arguments;

        /// <summary>
        /// Gets the formula arguments.
        /// </summary>
        public IReadOnlyList<FormulaArgument> Arguments => _arguments;
        public int MinArgsCount { get; private set; }
        public int MaxArgsCount { get; private set; }

        public FormulaInfo()
        {
            _arguments = new List<FormulaArgument>();
            MinArgsCount = 0;
            MaxArgsCount = 0;
        }

        public void AddArgument(FormulaArgument argument)
        {
            if (_arguments.Any(x => string.Equals(x.Name, argument.Name, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new AlphaXFormulaEngineException($"A formula argument with name '{argument.Name}' already exist.");
            }

            _arguments.Add(argument);

            if(argument.Required)
            {
                MinArgsCount++;
            }

            MaxArgsCount = _arguments.Count;
        }
    }
}
