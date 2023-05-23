
using System.Collections;
using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    public class ParseOrder : IEnumerable<ParseMode>
    {
        private HashSet<ParseMode> _order;

        public ParseOrder()
        {
            _order = new HashSet<ParseMode>();
        }

        public void Reset()
        {
            _order.Clear();
        }

        public void Add(ParseMode mode)
        {
            _order.Add(mode);
        }

        public IEnumerator<ParseMode> GetEnumerator()
        {
            return _order.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _order.GetEnumerator();
        }
    }
}