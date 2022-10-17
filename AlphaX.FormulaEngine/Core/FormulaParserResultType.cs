using AlphaX.Parserz;
using AlphaX.Parserz.Results;

namespace AlphaX.FormulaEngine
{
    internal class FormulaParserResultType
    {
        public static ParserResultType FormulaName = new ParserResultType("FormulaName");
    }

    internal class FormulaNameResult : ParserResult<string>
    {
        public FormulaNameResult(string value) : base(value, FormulaParserResultType.FormulaName)
        {
        }
    }
}
