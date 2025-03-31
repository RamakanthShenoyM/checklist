using System.Reflection;
using System.Text.Json;
using CommandEngine.Tasks;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using static CommandEngine.Commands.CommandReflection;

namespace CommandEngine.Commands {
    internal class EnvironmentSerializer : CommandVisitor {
        private ExtensionDto? _root;
        private readonly List<SimpleCommandDto> _simpleCommandDtos = [];
        private List<string> _events;
        private List<ContextEntryDto> _entries;

        public EnvironmentSerializer(CommandEnvironment environment) {
            environment.Accept(this);
        }

        internal string Result {
            get {
                if (_root == null)
                    throw new InvalidOperationException(
                        "The serializer has not examined the Command Environment. Visitor issue possible?");
                return JsonSerializer.Serialize(_root);
            }
        }

        public void Visit(Context c, Dictionary<Enum, object> entries, CommandHistory history) =>
            _entries = entries.Select(e => new ContextEntryDto(
                    e.Key.GetType().FullName ??
                    throw new InvalidOperationException("Missing required value for Context Label type in DTO"),
                    e.Key.ToString(),
                    e.Value.GetType().FullName ??
                    throw new InvalidOperationException("Missing required value for Context Value type in DTO"),
                    e.Value.ToString() ?? null))
                .ToList();

        public void Visit(CommandHistory history, List<string> events) => _events = events;

        public void PostVisit(CommandEnvironment environment,
            Guid environmentId,
            Guid clientId,
            Command command,
            Context c) =>
            _root = new ExtensionDto(environmentId.ToString(),
                clientId.ToString(),
                _simpleCommandDtos,
                _events,
                _entries);

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask) =>
            _simpleCommandDtos.Add(new SimpleCommandDto(state, Dto(executeTask), Dto(revertTask)));

        private TaskDto Dto(CommandTask task) {
            if (!task.GetType().NeedsMemento())
                return new TaskDto(task.GetType().FullName, "");
            var method = task.GetType().GetMethod("ToMemento", BindingFlags.Instance | BindingFlags.Public);
            var memento = method?.Invoke(task, null);
            return new TaskDto(task.GetType().FullName, (string)memento);
        }

        public record ExtensionDto(
            string EnvironmentId,
            string ClientId,
            List<SimpleCommandDto> SimpleCommandDtos,
            List<string> Events,
            List<ContextEntryDto> Entries) { }

        public record ContextEntryDto(string EnumType, string EnumValue, string ValueType, string ValueValue) { }

        public record TaskDto(string TaskType, string Memento) { }

        public record SimpleCommandDto(CommandState State, TaskDto ExecuteTask, TaskDto RevertTask) { }
    }
}