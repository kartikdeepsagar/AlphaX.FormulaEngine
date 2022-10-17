namespace AlphaX.FormulaEngine
{
    internal class EvaluationResult : IEvaluationResult
    {
        public object Value { get; }
        public string Error { get; }

        public EvaluationResult(object value)
        {
            Value = value;
        }

        public EvaluationResult(string error)
        {
            Error = error;
        }
    }
}
