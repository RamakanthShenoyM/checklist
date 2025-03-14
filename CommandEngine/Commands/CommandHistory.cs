
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandEventType;
namespace CommandEngine.Commands
{
    public class CommandHistory
    {
        private readonly List<CommandEvent> _events = new();

        internal CommandHistory() { }

		public List<CommandEvent> Events(CommandEventType type) => 
            _events.FindAll(e => e.EventType == type);
		internal void Event(Command command, CommandState originalState, CommandState newState) => 
            _events.Add(new CommandStateEvent(command, originalState, newState));

		internal void Event(SimpleCommand command, CommandTask task) => 
            _events.Add(new TaskStartedEvent(command, task));
	}

	internal class TaskStartedEvent(SimpleCommand command, CommandTask task) : CommandEvent
	{
        public CommandEventType EventType => TaskExecuted;

		public override string ToString() => $"Command <{command}> will execute Task <{task}>";
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
        TaskExecuted
    }
}