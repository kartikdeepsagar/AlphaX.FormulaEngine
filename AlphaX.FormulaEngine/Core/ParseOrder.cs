using System.Collections;
using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public class ParseOrder : IParseOrder
    {
        private HashSet<ParseType> _order;

        public ParseOrder(ParseType firstParseType)
        {
            _order = new HashSet<ParseType>();
            Add(firstParseType);
        }

        public void Add(ParseType mode)
        {
            _order.Add(mode);
        }

        public IEnumerator<ParseType> GetEnumerator()
        {
            return _order.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _order.GetEnumerator();
        }
    }
}