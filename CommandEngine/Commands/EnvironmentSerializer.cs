
using CommandEngine.Tasks;
using static CommandEngine.Tasks.CommandTask;

namespace CommandEngine.Commands
{
    internal class EnvironmentSerializer : CommandVisitor
    {
        private ExtensionDto _root;

        public EnvironmentSerializer(CommandEnvironment environment)
        {
            environment.Accept(this);
        }

        internal string Result => System.Text.Json.JsonSerializer.Serialize(_root);

        public void PreVisit(CommandEnvironment environment, Guid environmentId, Guid clientId, Command command, Context c)
        {
            _root = new ExtensionDto(environmentId.ToString(), clientId.ToString());
        }

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask)
        {
        }

        public class ExtensionDto(string environmentId, string clientId)
        {
            public string EnvironmentId { get; } = environmentId;
            public string ClientId { get; } = clientId;
        }

    }
}
