using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public class Context
    {
        private readonly Dictionary<Enum, object> _values = new();
        private readonly CommandHistory _history;
        private List<Enum>? _changedLabels;

        internal Context(List<string> events)
        {
            _history = new (events);
        }
        public Context():this([]) { }

        public object this[Enum label]
        {
            get
            {
                if (!_values.TryGetValue(label, out var item)) throw new MissingContextInformationException(label);
                return item;
            }
            set
            {
                if(value == null) throw new InvalidOperationException("Can't set a label to null; use Reset()");
                if (_changedLabels != null && !_changedLabels.Contains(label)) throw new UpdateNotCapturedException(label);
                _values[label] = value;
            }
        }
        public bool Has(Enum label) => _values.ContainsKey(label);

        public CommandHistory History => _history;

        public Context SubContext(List<Enum> labelsToCopy, List<Enum> changedLabels)
        {
            var result = new Context([]);
            foreach (var label in labelsToCopy) if (this.Has(label)) result[label] = this[label];
            result._changedLabels = changedLabels;
            return result;
        }

        internal void Update(Context subContext, List<Enum> changedLabels)
        {
            foreach (var label in changedLabels)
                if (subContext.Has(label)) this[label] = subContext[label];
            this.History.Update(subContext.History);
            
        }

        public override bool Equals(object? obj) =>
            this == obj || obj is Context other && this.Equals(other);

        public override int GetHashCode() => _history.GetHashCode();

		private bool Equals(Context other) => this._history.Equals(other._history) && this._values.SequenceEqual(other._values);

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

        internal void Event(SimpleCommand simpleCommand, CommandTask task, MissingContextInformationException e, object missingLabel) =>
            _history.Event(simpleCommand, task, e, missingLabel);
        internal void Event(SimpleCommand simpleCommand, CommandTask task, object changedLabel, UpdateNotCapturedException e) => 
            _history.Event(simpleCommand,  task, changedLabel, e);
        internal void Accept(CommandVisitor visitor)
        {
            visitor.Visit(this, _values, _history);
            _history.Accept(visitor);
        }

        public bool Reset(Enum label) => _values.Remove(label);
    }
}
