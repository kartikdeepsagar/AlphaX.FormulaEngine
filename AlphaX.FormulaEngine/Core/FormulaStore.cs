using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    internal class FormulaStore : IFormulaStore
    {
        private Dictionary<string, Formula> _formulas;

        public FormulaStore()
        {
            _formulas = new Dictionary<string, Formula>();
        }

        public IEnumerable<Formula> GetAll()
        {
            return _formulas.Values;
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
        }

        public void Remove(string formulaName)
        {
            if (_formulas.ContainsKey(formulaName))
                _formulas.Remove(formulaName);

            throw new AlphaXFormulaEngineException($"Invalid formula '{formulaName}'");
        }
    }
}
