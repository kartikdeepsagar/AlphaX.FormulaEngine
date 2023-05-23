using System.Collections;
using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public class ParseOrder : IEnumerable<ParseType>
    {
        private HashSet<ParseType> _order;

        public ParseOrder()
        {
            _order = new HashSet<ParseType>();
        }

        public void Reset()
        {
            _order.Clear();
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