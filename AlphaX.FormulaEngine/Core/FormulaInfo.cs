using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaX.FormulaEngine
{
    public class FormulaInfo
    {
        /// <summary>
        /// Gets the formula arguments.
        /// </summary>
        public List<FormulaArgument> Arguments { get; }
        public int MinArgsCount { get; private set; }
        public int MaxArgsCount { get; private set; }

        public FormulaInfo()
        {
            Arguments = new List<FormulaArgument>();
            MinArgsCount = 0;
            MaxArgsCount = 0;
        }

        public void AddArgument(FormulaArgument argument)
        {
            Arguments.Add(argument);

            if(argument.Required)
            {
                MinArgsCount++;
            }

            if(argument.IsArray)
            {
                MaxArgsCount = int.MaxValue;
            }
            else if(MaxArgsCount != int.MaxValue)
            {
                MaxArgsCount = Arguments.Count;
            }
        }
    }
}
