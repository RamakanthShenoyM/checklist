
using CommandEngine.Tasks;
using static CommandEngine.Tasks.CommandTask;

namespace CommandEngine.Commands
{
    internal class EnvironmentSerializer : CommandVisitor
    {
        private ExtensionDto _root;
        private readonly List<CommandState> _states = [];

        public EnvironmentSerializer(CommandEnvironment environment)
        {
            environment.Accept(this);
        }

        internal string Result => System.Text.Json.JsonSerializer.Serialize(_root);

        public void PostVisit(CommandEnvironment environment, Guid environmentId, Guid clientId, Command command, Context c)
        {
            _root = new ExtensionDto(environmentId.ToString(), clientId.ToString(), _states);
        }

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask) => _states.Add(state);

        public class ExtensionDto(string environmentId, string clientId, List<CommandState> states)
        {
            public string EnvironmentId { get; } = environmentId;
            public string ClientId { get; } = clientId;
            public List<CommandState> States { get; } = states;
        }

    }
}
