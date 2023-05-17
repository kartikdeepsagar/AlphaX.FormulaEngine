using System;

namespace AlphaX.FormulaEngine
{
    internal class FormulaEngineSettings : IEngineSettings
    {
        public bool DoubleQuotedStrings { get; set; }
        public string OpenBracketSymbol { get; set; }
        public string CloseBracketSymbol { get; set; }
        public string ArgumentsSeparatorSymbol { get; set; }

        public FormulaEngineSettings()
        {
            CloseBracketSymbol = ")";
            OpenBracketSymbol = "(";
            ArgumentsSeparatorSymbol = ",";
            DoubleQuotedStrings = true;
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

            ParserFactory.BuildParser(this);
        }
    }
}
