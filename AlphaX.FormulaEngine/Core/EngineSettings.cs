using System;
using System.Linq;

namespace AlphaX.FormulaEngine
{
    public class EngineSettings : IEngineSettings
    {
        public bool DoubleQuotedStrings { get; set; }
        public string OpenBracketSymbol { get; set; }
        public string CloseBracketSymbol { get; set; }
        public string ArgumentsSeparatorSymbol { get; set; }
        public IParseOrder EngineParseOrder { get; set; }
        public IParseOrder ArrayParseOrder { get; set; }
        public LogicalOperatorMode LogicalOperatorMode { get; set; }

        public EngineSettings()
        {
            LogicalOperatorMode = LogicalOperatorMode.Default;
            CloseBracketSymbol = SyntaxTokens.ClosedBracket;
            OpenBracketSymbol = SyntaxTokens.OpenBracket;
            ArgumentsSeparatorSymbol = SyntaxTokens.Comma;
            DoubleQuotedStrings = true;
            EngineParseOrder = ParseOrderBuilder.DefaultParseOrder;
            ArrayParseOrder = ParseOrderBuilder.DefaultArrayParseOrder;
        }
    }
}
