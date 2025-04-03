using CommandEngine.Commands;
using static CommandEngine.Commands.CommandEventType;

namespace CommandEngine.Tasks
{
    public class Context
    {
        private readonly Dictionary<Enum, object> _values = new();
        private readonly History _history;
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

        public History History => _history;

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
            this.History.Update(subContext.History, StartSubTaskHistory, EndSubTaskHistory);
            
        }

        public override bool Equals(object? obj) =>
            this == obj || obj is Context other && this.Equals(other);

        public override int GetHashCode() => _history.GetHashCode();

		private bool Equals(Context other) => this._history.Equals(other._history) && this._values.SequenceEqual(other._values);

		internal void Event(SimpleCommand command, CommandState originalState, CommandState newState) => 
            _history.Add(CommandStateChange, $"Command <{command}> Changed State from <{originalState}> To <{newState}>");

        internal void Event(SimpleCommand command, CommandTask task, object conclusion) =>
            _history.Add(ConclusionReached, $"Task <{task}> reached a conclusion<{conclusion}>");

        internal void Event(SimpleCommand command, CommandTask task, CommandStatus status)
        {
            var statusMsg = (task is IgnoreTask) ?
                $"Command <{command}> has no Undo"
                : $"Task <{task}> completed with status <{status}>";
            _history.Add(CommandEventType.TaskStatus, statusMsg);
        }

        internal void Event(SimpleCommand command, CommandTask task, Exception e) =>
             _history.Add(TaskException, $"Task <{task}> threw an exception <{e}>");
        internal void Event(SimpleCommand command, CommandTask task, object label, object? previousValue, object? newValue) =>
            _history.Add(ValueChanged, 
                $"Task <{task}> in Command <{command}> changed <{label}> from <{previousValue}> to <{newValue}>");
        internal void StartEvent(SerialCommand command) =>
             _history.Add(GroupSerialStart, $"Group Command <{command.NameOnly()}> started");
        internal void CompletedEvent(SerialCommand command) =>
            _history.Add(GroupSerialComplete, $"Group Command <{command.NameOnly()}> completed");
        internal void Event(SimpleCommand simpleCommand, CommandTask task, MissingContextInformationException e, object missingLabel) =>
            _history.Add(InvalidAccessAttempt, $"Invalid Access to <{missingLabel}> by <{task}>");
        internal void Event(SimpleCommand simpleCommand, CommandTask task, object changedLabel, UpdateNotCapturedException e) => 
            _history.Add(UpdateNotCaptured,  $"Attempt to set <{changedLabel}> in the context, but not marked as a change field");
        internal void Accept(CommandVisitor visitor)
        {
            visitor.Visit(this, _values, _history);
            _history.Accept(visitor);
        }

        public bool Reset(Enum label) => _values.Remove(label);

        internal Context Merge(Context other)
        {
            this.History.Merge(other.History);
            return this;
        }
    }
}
