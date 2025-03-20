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

        public override string ToString() => string.Join("\n", _events);

        public List<string> Events(string type) => 
            _events.FindAll(e => e.Contains(type));

		internal void Event(SimpleCommand command, CommandState originalState, CommandState newState) => 
            _events.Add($"{DateTime.Now} >> {CommandStateChange} << Status: Command <{command}> Changed State from <{originalState}> To <{newState}>");

		internal void Event(SimpleCommand command, CommandTask task, CommandStatus status)
		{
            var statusMsg = (task is IgnoreTask) ?
            $"Command <{command}> has no Undo"
            : $"Task <{task}> completed with status <{status}>";
			_events.Add($"{DateTime.Now} >> {CommandEventType.TaskStatus} << Status: {statusMsg}");
		}

		internal void Event(SimpleCommand command, CommandTask task, Exception e) => 
            _events.Add($"{DateTime.Now} >> {TaskException} << Status: Task <{task}> threw an exception <{e}>");
        internal void Event(SimpleCommand command, CommandTask task, object conclusion) => 
            _events.Add($"{DateTime.Now} >> {ConclusionReached} << Status: Task <{task}> reached a conclusion<{conclusion}>");


        public override bool Equals(object? obj) =>
            this == obj || obj is CommandHistory other && this.Equals(other);

        public override int GetHashCode() => string.Join("", this._events).GetHashCode();
        private bool Equals(CommandHistory other) =>
            this._events.SequenceEqual(other._events);

        internal void Event(SimpleCommand command, CommandTask task) => 
            _events.Add($"{DateTime.Now} >> {CommandEventType.TaskExecuted} << Status: Starting Command <{command}>, executing Task <{task}>");

        internal void Event(SimpleCommand command, CommandTask task, object label, object? previousValue, object? newValue) => _events.Add($"{DateTime.Now} >> {ValueChanged} << Status: Task <{task}> in Command <{command}> changed <{label}> from <{previousValue}> to <{newValue}>");

        internal void StartEvent(SerialCommand command) => 
            _events.Add($"{DateTime.Now} >> {GroupSerialStart} << Status: Group Command <{command.NameOnly()}> started");

        internal void CompletedEvent(SerialCommand command) =>
            _events.Add($"{DateTime.Now} >> {GroupSerialComplete} << Status: Group Command <{command.NameOnly()}> completed");

        internal void Accept(CommandVisitor visitor) => visitor.Visit(this,_events);
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
        ConclusionReached
    }
}