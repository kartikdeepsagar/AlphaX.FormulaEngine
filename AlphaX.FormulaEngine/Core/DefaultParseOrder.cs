namespace AlphaX.FormulaEngine
{
    internal class DefaultParseOrder : ParseOrder
    {
        public DefaultParseOrder()
        {
            Reset();
            Add(ParseType.Array);
            Add(ParseType.CustomName);
            Add(ParseType.String);
            Add(ParseType.Number);
            Add(ParseType.Boolean);
            Add(ParseType.Formula);
        }
    }
}