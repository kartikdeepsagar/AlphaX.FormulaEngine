namespace AlphaX.FormulaEngine
{
    public static class ParseOrderBuilder
    {
        public static IParseOrder DefaultParseOrder { get; }
        public static IParseOrder DefaultArrayParseOrder { get; }

        static ParseOrderBuilder()
        {
            DefaultParseOrder = FirstParse(ParseType.Number)
                .AndThenParse(ParseType.Array)
                .AndThenParse(ParseType.String)
                .AndThenParse(ParseType.Boolean)
                .AndThenParse(ParseType.CustomName)
                .AndThenParse(ParseType.Formula);

            DefaultArrayParseOrder = FirstParse(ParseType.Number)
                .AndThenParse(ParseType.String)
                .AndThenParse(ParseType.Boolean)
                .AndThenParse(ParseType.CustomName)
                .AndThenParse(ParseType.Formula);
        }

        public static IParseOrder FirstParse(ParseType firstParse)
        {
            return new ParseOrder(firstParse);
        }

        public static IParseOrder AndThenParse(this IParseOrder parseOrder, ParseType parseType)
        {
            (parseOrder as ParseOrder).Add(parseType);
            return parseOrder;
        }
    }
}