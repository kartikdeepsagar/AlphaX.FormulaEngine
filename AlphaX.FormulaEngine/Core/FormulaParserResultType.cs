using AlphaX.Parserz;
using AlphaX.Parserz.Results;

namespace AlphaX.FormulaEngine
{
    internal class FormulaParserResultType
    {
        public static ParserResultType FormulaName = new ParserResultType("FormulaName");
        public static ParserResultType FormulaBracket = new ParserResultType("FormulaBracket");
        public static ParserResultType FormulaArgumentSeperator = new ParserResultType("FormulaArgumentSeperator");
    }

    internal class FormulaNameResult : ParserResult<string>
    {
        public FormulaNameResult(string value) : base(value, FormulaParserResultType.FormulaName)
        {
        }
    }

    internal class FormulaBracketResult : ParserResult<string>
    {
        public FormulaBracketResult(string value) : base(value, FormulaParserResultType.FormulaBracket)
        {
        }
    }

    internal class FormulaArgumentSeperatorResult : ParserResult<string>
    {
        public FormulaArgumentSeperatorResult(string value) : base(value, FormulaParserResultType.FormulaArgumentSeperator)
        {
        }
    }
}
