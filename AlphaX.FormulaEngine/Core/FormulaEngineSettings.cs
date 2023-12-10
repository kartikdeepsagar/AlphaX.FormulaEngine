using System;
using System.Linq;
using AlphaX.Parserz;

namespace AlphaX.FormulaEngine
{
    internal class FormulaEngineSettings : IEngineSettings
    {
        public bool DoubleQuotedStrings { get; set; }
        public string OpenBracketSymbol { get; set; }
        public string CloseBracketSymbol { get; set; }
        public string ArgumentsSeparatorSymbol { get; set; }
        public IParseOrder EngineParseOrder { get; set; }
        public IParseOrder ArrayParseOrder { get; set; }

        public FormulaEngineSettings()
        {
            CloseBracketSymbol = Tokens.ClosedBracket;
            OpenBracketSymbol = Tokens.OpenBracket;
            ArgumentsSeparatorSymbol = Tokens.Comma;
            DoubleQuotedStrings = true;
            EngineParseOrder = ParseOrderBuilder.DefaultParseOrder;
            ArrayParseOrder = ParseOrderBuilder.DefaultArrayParseOrder;
            Save();
        }

        public void Save()
        {
            if (OpenBracketSymbol is null)
                throw new NotSupportedException("Open bracket symbol cannot be null");

            if (CloseBracketSymbol is null)
                throw new NotSupportedException("Close bracket symbol cannot be null");

            if (ArgumentsSeparatorSymbol is null)
                throw new NotSupportedException("Argument separator symbol cannot be null");

            if (EngineParseOrder is null || EngineParseOrder.Count() == 0)
                throw new InvalidOperationException("Invalid engine parse order");

            if (ArrayParseOrder is null || ArrayParseOrder.Count() == 0)
                throw new InvalidOperationException("Invalid array parse order");

            ParserFactory.BuildParser(this);
        }
    }
}
