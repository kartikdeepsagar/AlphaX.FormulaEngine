namespace AlphaX.FormulaEngine
{
    internal class DefaultParseOrder : ParseOrder
    {
        public DefaultParseOrder()
        {
            Reset();
            Add(ParseType.Number);
            Add(ParseType.Array);
            Add(ParseType.String);
            Add(ParseType.Boolean);
            Add(ParseType.CustomName);
            Add(ParseType.Formula);
        }
    }

    internal class DefaultArrayParseOrder : ParseOrder
    {
        public DefaultArrayParseOrder()
        {
            Reset();
            Add(ParseType.Number);
            Add(ParseType.String);
            Add(ParseType.Boolean);
            Add(ParseType.CustomName);
            Add(ParseType.Formula);
        }
    }
}