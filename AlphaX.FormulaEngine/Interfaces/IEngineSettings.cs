namespace AlphaX.FormulaEngine
{
    public interface IEngineSettings
    {
        /// <summary>
        /// Gets or sets whether the engine supports single/double quote for string inputs. (Default is true).
        /// </summary>
        bool DoubleQuotedStrings { get; set; }
        /// <summary>
        /// Gets or sets the argument seperator symbol. Default is comma (',').
        /// </summary>
        string ArgumentsSeparatorSymbol { get; set; }
        /// <summary>
        /// Gets or sets the formula close bracket symbol.
        /// </summary>
        string CloseBracketSymbol { get; set; }
        /// <summary>
        /// Gets or sets the formula open brakcet symbol.
        /// </summary>
        string OpenBracketSymbol { get; set; }
        /// <summary>
        /// Gets or sets the parse order for engine.
        /// </summary>
        IParseOrder EngineParseOrder { get; set; }
        /// <summary>
        /// Gets or sets the parse order for array values.
        /// </summary>
        IParseOrder ArrayParseOrder { get; set; }
        /// <summary>
        /// Saves the settings.
        /// </summary>
        void Save();
    }
}
