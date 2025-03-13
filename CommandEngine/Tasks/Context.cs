
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

        public Context SubContext(params object[] labels)
        {
            var result = new Context();
            foreach (var label in labels)
            {
                if (!_values.ContainsKey(label)) throw new MissingContextInformationException(label);
                result[label] = _values[label];
            }
            return result;
        }
	}
}
