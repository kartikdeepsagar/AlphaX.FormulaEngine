namespace AlphaX.FormulaEngine
{
    internal class DefaultParseOrder : ParseOrder
    {
        public DefaultParseOrder()
        {
            Reset();
            Add(ParseMode.Array);
            Add(ParseMode.CustomName);
            Add(ParseMode.String);
            Add(ParseMode.Number);
            Add(ParseMode.Boolean);
            Add(ParseMode.Formula);
        }
    }
}