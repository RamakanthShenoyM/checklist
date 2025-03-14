
using static CommandEngine.Commands.CommandEventType;
namespace CommandEngine.Commands
{
    public class CommandHistory
    {
        private readonly Dictionary<CommandEventType, List<CommandEvent>> _events = new();
        internal CommandHistory() { }

        public List<CommandEvent> Events(CommandEventType type)
        {
            if(!_events.ContainsKey(type)) _events[type] = new();
            return _events[type];
        }
        internal void Event(Command command, CommandState originalState, CommandState newState)
        {
            Events(CommandStateChange).Add(new CommandStateEvent(command, originalState, newState));
        }
    }

    internal class CommandStateEvent(Command command, CommandState originalState, CommandState newState) : CommandEvent
    {
        public override string ToString() => $"Command <{command}> Changed State from <{originalState}> To <{newState}>";
    }

    public interface CommandEvent
    {

    }
    public enum CommandEventType
    {
        CommandStateChange
    }
}