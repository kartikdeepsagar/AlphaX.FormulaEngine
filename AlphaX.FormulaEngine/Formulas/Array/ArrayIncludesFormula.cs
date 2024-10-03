using System;
using System.Collections;

namespace AlphaX.FormulaEngine.Formulas
{
    public class ArrayIncludesFormula : Formula
    {
        public ArrayIncludesFormula() : base("ARRAYINCLUDES")
        {
        }

        public override object Evaluate(params object[] args)
        {
            var sourceArray = (object[])args[0];
            var targetArray = (object[])args[1];

            for(int index = 0; index < targetArray.Length; index++)
            {
                var item = targetArray[index];
                if(!Array.Exists(sourceArray, x => Comparer.Equals(x, item)))
                {
                    return false;
                }
            }

            return true;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Checks if array includes all values."
            };
            info.AddArgument(new ArrayArgument("source", true)
            {
                Description = "Source array to search in."
            });
            info.AddArgument(new ArrayArgument("values", true)
            {
                Description = "Values to search for."
            });
            return info;
        }
    }
}
