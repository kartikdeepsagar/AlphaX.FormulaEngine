using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class OperatorFormula : Formula
    {
        protected Func<string> _getOperator;

        internal OperatorFormula(string name, Func<string> getOperator) : base(name)
        {
            _getOperator = getOperator;
        }

        public override object Evaluate(params object[] args)
        {
            return Engine.Evaluator.Compare(args[0], _getOperator(), args[1]);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = $"Performs '{Name}' logical operation between two operands."
            };
            info.AddArgument(new ObjectArgument("value1", true)
            {
                Description = "The first operand."
            });
            info.AddArgument(new ObjectArgument("value2", true)
            {
                Description = "The second operand."
            });
            return info;
        }
    }
}
