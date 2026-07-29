using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace DevBrewLabs.Evalis
{
    internal class FormulaStore : IFormulaStore
    {
        private ConcurrentDictionary<string, FormulaBase> _formulas;
        private FormulaEngine _formulaEngine;

        public FormulaStore(FormulaEngine engine)
        {
            _formulas = new ConcurrentDictionary<string, FormulaBase>(StringComparer.OrdinalIgnoreCase);
            _formulaEngine = engine;
        }

        public IEnumerable<FormulaInfo> GetAll()
        {
            return _formulas.Select(x => x.Value.Info);
        }

        public FormulaBase Get(string formulaName)
        {
            return _formulas[formulaName];
        }

        public bool TryGet(string formulaName, out FormulaBase formula)
        {
            return _formulas.TryGetValue(formulaName, out formula);
        }

        public bool Contains(string formulaName)
        {
            return _formulas.ContainsKey(formulaName);
        }

        public void Add(FormulaBase formula)
        {
            _formulas.TryAdd(formula.Name, formula);
        }

        public void Remove(string formulaName)
        {
            if (!_formulas.TryRemove(formulaName, out _))
            {
                throw new InvalidOperationException($"Invalid formula '{formulaName}'");
            }
        }
    }
}
