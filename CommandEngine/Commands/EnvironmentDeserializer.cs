using CommandEngine.Tasks;
using static CommandEngine.Commands.EnvironmentSerializer;

namespace CommandEngine.Commands
{
    internal class EnvironmentDeserializer : CommandVisitor
    {
        private CommandEnvironment _environment;
        private List<CommandState> _states;

        public EnvironmentDeserializer(string json)
        {
            var dto = System.Text.Json.JsonSerializer.Deserialize<ExtensionDto>(json) ?? throw new InvalidOperationException("Invalid Json");
            _states = dto.States;
            _environment =  CommandEnvironment.RestoredEnvironment(CommandEnvironment.Environment(new Guid(dto.EnvironmentId)), new Guid(dto.ClientId), new Tasks.Context());
            _environment.Accept(this);
        }

        public CommandEnvironment Result => _environment;

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask)
        {
            command.State(_states[0]);
            _states.RemoveAt(0);
        }
    }
}