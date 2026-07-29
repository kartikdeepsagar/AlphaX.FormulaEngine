using System;
using System.Linq;
using System.Collections.Generic;
using DevBrewLabs.Parserly;

namespace DevBrewLabs.Evalis
{
    /// <summary>
    /// Default implementation of IEngineSettings with sensible defaults: double-quoted strings enabled, default logical operators, and the standard parse order.
    /// </summary>
    public class EngineSettings : IEngineSettings
    {
        public bool DoubleQuotedStrings { get; set; }
        public IParseOrder EngineParseOrder { get; set; }
        public LogicalOperatorMode LogicalOperatorMode { get; set; }
        public List<IParser> CustomTokenParsers { get; set; }
        public bool EnableCaching { get; set; }

        /// <summary>
        /// Initializes EngineSettings with default values.
        /// </summary>
        public EngineSettings()
        {
            LogicalOperatorMode = LogicalOperatorMode.Default;
            DoubleQuotedStrings = true;
            EngineParseOrder = ParseOrderBuilder.DefaultParseOrder;
            EnableCaching = true;
        }
    }
}
