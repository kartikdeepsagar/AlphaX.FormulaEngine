using System;

namespace AlphaX.FormulaEngine
{
    public class EvaluationException : Exception
    {
        public EvaluationException(string message) : base(message)
        {

        }
    }

    public class AlphaXFormulaEngineException : Exception
    {
        public AlphaXFormulaEngineException(string message) : base(message)
        {

        }
    }
}
