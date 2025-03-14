


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

        public bool Has(object label) => _values.ContainsKey(label);

        public Context SubContext(List<object> labels)
        {
            var result = new Context();
            foreach (var label in labels) if (this.Has(label)) result[label] = this[label];
            return result;
        }

        internal void Update(Context subContext, List<object> changedLabels)
        {
            foreach (var label in changedLabels) 
                if (subContext.Has(label)) this[label] = subContext[label];
        }
    }
}
