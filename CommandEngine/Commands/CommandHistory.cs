
using System.Reflection.Emit;
using System.Threading.Tasks;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandEventType;
namespace CommandEngine.Commands
{
    public class CommandHistory
    {
        private readonly List<CommandEvent> _events = new();

        internal CommandHistory() { }

        override public string ToString() => string.Join("\n", _events);

        public List<CommandEvent> Events(CommandEventType type) => 
            _events.FindAll(e => e.EventType == type);
		internal void Event(Command command, CommandState originalState, CommandState newState) => 
            _events.Add(new CommandStateEvent(command, originalState, newState));

		internal void Event(SimpleCommand command, CommandTask task) => 
            _events.Add(new TaskStartedEvent(command, task));

        internal void Event(SimpleCommand command, CommandTask task, object label, object? previousValue, object? newValue)
        {
            _events.Add(new ValueChangedEvent(command, task,label,previousValue,newValue));

        }
        internal void Event(SerialCommand command)
        {
            _events.Add(new GroupSerialStartEvent(command));
        }
    }

    internal class GroupSerialStartEvent(SerialCommand command) : CommandEvent
    {
        public CommandEventType EventType => GroupSerialStart;
        public override string ToString() => $"Group Command <{command}> started";
    }

    internal class ValueChangedEvent(SimpleCommand command, CommandTask task, object label, object? previousValue, object? newValue) : CommandEvent
    {
        public CommandEventType EventType => ValueChanged;
        public override string ToString() => $"Task <{task}> in Command <{command}> changed <{label}> from <{previousValue}> to <{newValue}>";
    }

    internal class TaskStartedEvent(SimpleCommand command, CommandTask task) : CommandEvent
	{
        public CommandEventType EventType => TaskExecuted;

		public override string ToString() => $"Starting Command <{command}>, executing Task <{task}>";
	}

	internal class CommandStateEvent(Command command, CommandState originalState, CommandState newState) : CommandEvent
    {
		public CommandEventType EventType => CommandStateChange;

		public override string ToString() => $"Command <{command}> Changed State from <{originalState}> To <{newState}>";
    }

    public interface CommandEvent
    {
        CommandEventType EventType { get; }

    }
    public enum CommandEventType
    {
        CommandStateChange,
        TaskExecuted,
        ValueChanged,
        GroupSerialStart
    }
}