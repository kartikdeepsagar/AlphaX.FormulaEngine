using AlphaX.Parserz;

namespace AlphaX.FormulaEngine
{
    internal class FormulaParserResultType
    {
        public static ParserResultType FormulaName = new ParserResultType("FormulaName");
        public static ParserResultType FormulaBracket = new ParserResultType("FormulaBracket");
        public static ParserResultType FormulaArgumentSeperator = new ParserResultType("FormulaArgumentSeperator");
        public static ParserResultType Operator = new ParserResultType("Operator");
        public static ParserResultType Condition = new ParserResultType("Condition");
        public static ParserResultType CustomName = new ParserResultType("CustomName");
    }

    public struct Condition
    {
        public object LeftOperand { get; set; }
        public string Operator { get; set; }
        public object RightOperand { get; set; }

        public Condition(object leftOperand, string @operator, object rightOperand)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }
    }

    public struct CustomName
    {
        public string Value { get; set; }

        public CustomName(string value)
        {
            Value = value;
        }
    }

    internal class ConditionResult : ParserResult<Condition>
    {
        public ConditionResult(Condition value) : base(value, FormulaParserResultType.Condition)
        {

        }
    }

    internal class CustomNameResult : ParserResult<CustomName>
    {
        public CustomNameResult(CustomName value) : base(value, FormulaParserResultType.CustomName)
        {

        }
    }

    internal class OperatorResult : ParserResult<string>
    {
        public OperatorResult(string value) : base(value, FormulaParserResultType.Operator)
        {

        }
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
