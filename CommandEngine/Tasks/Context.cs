using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandEngine.Tasks
{
    public class Context
    {
        private readonly Dictionary<object, object> _values = new();
        public object this[object label]
        {
            get
            {
                if (!_values.ContainsKey(label)) throw new MissingContextInformationException(label);
                return _values[label];
            }
            set
            {
                _values[label] = value;
            }
        }
    }
}
