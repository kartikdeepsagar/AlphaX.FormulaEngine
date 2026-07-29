using DevBrewLabs.Parserly;
using System.Collections.Generic;

namespace DevBrewLabs.Evalis
{
    internal class FormulaParserResultType
    {
        public static ParserResultType Formula = new ParserResultType("Formula");
        public static ParserResultType Operator = new ParserResultType("Operator");
        public static ParserResultType OpenBracket = new ParserResultType("OpenBracket");
        public static ParserResultType CloseBracket = new ParserResultType("CloseBracket");
        public static ParserResultType CustomName = new ParserResultType("CustomName");
    }

    internal struct CustomName
    {
        public string Value { get; set; }

        public CustomName(string value)
        {
            Value = value;
        }
    }

    internal struct FormulaExpr
    {
        public string Name { get; set; }
        public IParserResult[] Args { get; set; }

        public FormulaExpr(string name, IParserResult[] args)
        {
            Name = name;
            Args = args;
        }
    }

    internal class CustomNameResult : ParserResult<CustomName>
    {
        public CustomNameResult(CustomName value) : base(value, FormulaParserResultType.CustomName) { }
    }

    internal class OpenBracketResult : ParserResult<string>
    {
        public OpenBracketResult() : base(SyntaxTokens.OpenBracket, FormulaParserResultType.OpenBracket) { }
    }

    internal class CloseBracketResult : ParserResult<string>
    {
        public CloseBracketResult() : base(SyntaxTokens.ClosedBracket, FormulaParserResultType.CloseBracket) { }
    }

    internal class FormulaResult : ParserResult<FormulaExpr>
    {
        public FormulaResult(FormulaExpr value) : base(value, FormulaParserResultType.Formula) { }
    }

    internal class OperatorResult : ParserResult<string>
    {
        public List<IParserResult> Children { get; }
        public OperatorResult(string value) : base(value, FormulaParserResultType.Operator) {
            Children = new List<IParserResult>();
        }
    }
}
