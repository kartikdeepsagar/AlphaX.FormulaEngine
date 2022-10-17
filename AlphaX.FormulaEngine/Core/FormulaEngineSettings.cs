using System;

namespace AlphaX.FormulaEngine
{
    public class FormulaEngineSettings
    {
        /// <summary>
        /// Gets the default settings.
        /// </summary>
        public static FormulaEngineSettings Default => new FormulaEngineSettings()
        {
            CloseBracketSymbol = ")",
            OpenBracketSymbol = "(",
            ArgumentsSeparatorSymbol = ",",
        };

        /// <summary>
        /// Gets or sets the formula open brakcet symbol.
        /// </summary>
        public string OpenBracketSymbol { get; set; }
        /// <summary>
        /// Gets or sets the formula close bracket symbol.
        /// </summary>
        public string CloseBracketSymbol { get; set; }
        /// <summary>
        /// Gets or sets the argument seperator symbol. Default is comma (',').
        /// </summary>
        public string ArgumentsSeparatorSymbol { get; set; }

        /// <summary>
        /// Re-inititalizes the engine with the updated settings.
        /// </summary>
        public void Update()
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
