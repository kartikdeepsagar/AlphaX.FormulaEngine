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
        public ParseOrder ParseOrder { get; set; }

        public FormulaEngineSettings()
        {
            CloseBracketSymbol = Tokens.ClosedBracket;
            OpenBracketSymbol = Tokens.OpenBracket;
            ArgumentsSeparatorSymbol = Tokens.Comma;
            DoubleQuotedStrings = true;
            ParseOrder = new DefaultParseOrder();
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

            if (ParseOrder is null || ParseOrder.Count() == 0)
                throw new InvalidOperationException("Invalid parser order");

            ParserFactory.BuildParser(this);
        }
    }
}
