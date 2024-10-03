using System.Collections.Generic;
using System.Linq;

namespace AlphaX.FormulaEngine
{
    internal class FormulaStore : IFormulaStore
    {
        private Dictionary<string, Formula> _formulas;
        private AlphaXFormulaEngine _formulaEngine;

        public FormulaStore(AlphaXFormulaEngine engine)
        {
            _formulas = new Dictionary<string, Formula>();
            _formulaEngine = engine;
        }

        public IEnumerable<FormulaInfo> GetAll()
        {
            return _formulas.Select(x => x.Value.Info);
        }

        public Formula Get(string formulaName)
        {
            return _formulas[formulaName];
        }

        public bool Contains(string formulaName)
        {
            return _formulas.ContainsKey(formulaName);
        }

        public void Add(Formula formula)
        {
            _formulas.Add(formula.Name, formula);
            formula.Engine = _formulaEngine;
        }

        public void Remove(string formulaName)
        {
            if (_formulas.ContainsKey(formulaName))
            {
                _formulas[formulaName].Engine = null;
                _formulas.Remove(formulaName);
            }

            throw new AlphaXFormulaEngineException($"Invalid formula '{formulaName}'");
        }
    }
}
