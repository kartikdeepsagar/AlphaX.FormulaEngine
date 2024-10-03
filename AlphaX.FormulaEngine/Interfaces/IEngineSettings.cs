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
        /// Gets or sets the formula close bracket symbol. Default is ')'.
        /// </summary>
        string CloseBracketSymbol { get; set; }
        /// <summary>
        /// Gets or sets the formula open brakcet symbol.  Default is '('.
        /// </summary>
        string OpenBracketSymbol { get; set; }
        /// <summary>
        /// Gets or sets the parse order for engine. The engine will try to parse formula argument in the specified order. This could be used to improve engine performance. For example, Number could be specified first in parse order if the formulas to be used only uses numeric arguments.
        /// </summary>
        IParseOrder EngineParseOrder { get; set; }
        /// <summary>
        /// Gets or sets the parse order for array values. The engine will try to parse array values in the specified order.
        /// </summary>
        IParseOrder ArrayParseOrder { get; set; }
        /// <summary>
        /// Gets or sets the logical operator mode. For example, 'eq' instead of '=', 'ne' instead of '!=' etc.
        /// </summary>
        LogicalOperatorMode LogicalOperatorMode { get; set; }
    }
}
