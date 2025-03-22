using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandEventType;
namespace CommandEngine.Commands
{
    public class CommandHistory
    {
        private readonly List<string> _events;

        internal CommandHistory(List<string> events)
        {
            _events = events;
        }
        
        public override bool Equals(object? obj) =>
            this == obj || obj is CommandHistory other && this.Equals(other);
        
        private bool Equals(CommandHistory other) =>
            this._events.SequenceEqual(other._events);

        public override int GetHashCode() => string.Join("", this._events).GetHashCode();

        public override string ToString() => string.Join("\n", _events);
        
        internal void Accept(CommandVisitor visitor) => visitor.Visit(this,_events);

        public List<string> Events(CommandEventType type) => 
            _events.FindAll(e => e.Contains($">> {type} <<"));
		
        internal void Event(SimpleCommand command, CommandState originalState, CommandState newState) => 
            _events.Add(Header(CommandStateChange) + $"Command <{command}> Changed State from <{originalState}> To <{newState}>");

		internal void Event(SimpleCommand command, CommandTask task, CommandStatus status)
		{
            var statusMsg = (task is IgnoreTask) ?
            $"Command <{command}> has no Undo"
            : $"Task <{task}> completed with status <{status}>";
			_events.Add(Header(CommandEventType.TaskStatus) + statusMsg);
		}

		internal void Event(SimpleCommand command, CommandTask task, Exception e) => 
            _events.Add(Header(TaskException) + $"Task <{task}> threw an exception <{e}>");
        
        internal void Event(SimpleCommand command, CommandTask task, object conclusion) => 
            _events.Add(Header(ConclusionReached) + $"Task <{task}> reached a conclusion<{conclusion}>");

        internal void Event(SimpleCommand command, CommandTask task) => 
            _events.Add(Header(TaskExecuted) + $"Starting Command <{command}>, executing Task <{task}>");

        internal void Event(SimpleCommand command, CommandTask task, object label, object? previousValue, object? newValue) => 
            _events.Add(Header(ValueChanged) + $"Task <{task}> in Command <{command}> changed <{label}> from <{previousValue}> to <{newValue}>");
        
        internal void StartEvent(SerialCommand command) => 
            _events.Add(Header(GroupSerialStart) + $"Group Command <{command.NameOnly()}> started");

        internal void CompletedEvent(SerialCommand command) => 
            _events.Add(Header(GroupSerialComplete) + $"Group Command <{command.NameOnly()}> completed");

        internal void Event(SimpleCommand simpleCommand, CommandTask task, MissingContextInformationException e, object missingLabel) => 
            _events.Add(Header(InvalidAccessAttempt) + $"Invalid Access to <{missingLabel}> by <{task}>");

        internal void Event(SimpleCommand simpleCommand, CommandTask task, object changedLabel, UpdateNotCapturedException e) => 
            _events.Add(Header(UpdateNotCaptured) + $"Attempt to set <{changedLabel}> in the context, but not marked as a change field");

        private static string Header(CommandEventType type) => $"{DateTime.Now} >> {type} << Status: ";
    }

    public enum CommandEventType
    {
        CommandStateChange,
        TaskExecuted,
        ValueChanged,
        GroupSerialStart,
        GroupSerialComplete,
        TaskException,
        TaskStatus,
        ConclusionReached,
        InvalidAccessAttempt,
        UpdateNotCaptured
    }
}