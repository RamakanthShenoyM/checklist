using System.Text.Json;
using CommandEngine.Tasks;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace CommandEngine.Commands
{
    internal class EnvironmentSerializer : CommandVisitor
    {
        private ExtensionDto? _root;
        private readonly List<CommandState> _states = [];
        private List<string> _events;
        private List<ContextEntryDto> _entries;

        public EnvironmentSerializer(CommandEnvironment environment)
        {
            environment.Accept(this);
        }

        internal string Result {
            get {
                if (_root == null) throw new InvalidOperationException(
                        "The serializer has not examined the Command Environment. Visitor issue possible?");
                return JsonSerializer.Serialize(_root);
            }
        }

        public void Visit(Context c,Dictionary<Enum, object> entries,CommandHistory history)
		{
			_entries = entries.Select(e => new ContextEntryDto(e.Key.GetType().FullName, e.Key.ToString(), e.Value.GetType().FullName, e.Value.ToString())).ToList();
		}

		public void Visit(CommandHistory history, List<string> events) => _events = events;

        public void PostVisit(CommandEnvironment environment, Guid environmentId, Guid clientId, Command command, Context c) => 
            _root = new ExtensionDto(environmentId.ToString(), clientId.ToString(), _states,_events, _entries);

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask) => 
            _states.Add(state);

        public class ExtensionDto(string environmentId, string clientId, List<CommandState> states,List<string> events, List<ContextEntryDto> entries)
        {
            public string EnvironmentId { get; } = environmentId;
            public string ClientId { get; } = clientId;
            public List<CommandState> States { get; } = states;
            public List<string> Events { get; } = events;
			public List<ContextEntryDto> Entries { get; } = entries;
		}

		public class ContextEntryDto(string enumType, string enumValue, string valueType, string valueValue)
		{
			public string EnumType { get; } = enumType;
			public string EnumValue { get; } = enumValue;
			public string ValueType { get; } = valueType;
			public string ValueValue { get; } = valueValue;
		}
	}
}
