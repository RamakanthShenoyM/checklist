using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public class Context
    {
        private readonly Dictionary<object, object> _values = new();
        private readonly CommandHistory _history = new();

        public object this[object label]
        {
            get
            {
                if (!_values.TryGetValue(label, value: out var item)) throw new MissingContextInformationException(label);
                return item;
            }
            set => _values[label] = value;
        }

        public bool Has(object label) => _values.ContainsKey(label);

        public CommandHistory History => _history;

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

        internal void Event(SimpleCommand command, CommandState originalState, CommandState newState) => 
            _history.Event(command, originalState, newState);
        internal void Event(SimpleCommand command, CommandTask task, object conclusion) =>
            _history.Event(command, task, conclusion);

        internal void Event(SimpleCommand command, CommandTask task, CommandStatus status) => 
            _history.Event(command, task, status);

        internal void Event(SimpleCommand command, CommandTask task, Exception e) => 
            _history.Event(command, task, e);

        internal void Event(SimpleCommand command, CommandTask task, object label, object? previousValue, object? newValue) => 
            _history.Event(command, task, label,previousValue,newValue);

        internal void StartEvent(SerialCommand command) => _history.StartEvent(command);

        internal void CompletedEvent(SerialCommand command) => _history.CompletedEvent(command);

        internal void Accept(CommandVisitor visitor) => visitor.Visit(this, _history);
    }
}
