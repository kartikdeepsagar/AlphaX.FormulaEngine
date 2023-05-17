﻿using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public interface IFormulaEngine
    {
        /// <summary>
        /// Gets the engine context.
        /// </summary>
        IEngineContext Context { get; set; }
        /// <summary>
        /// Gets the engine settings.
        /// </summary>
        IEngineSettings Settings { get; }
        /// <summary>
        /// Gets the formula store.
        /// </summary>
        IFormulaStore FormulaStore { get; }
        /// <summary>
        /// Gets the evaluated result of the provided formula expression.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEvaluationResult Evaluate(string input);
    }
}
