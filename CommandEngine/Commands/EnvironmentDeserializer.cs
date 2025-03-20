using CommandEngine.Tasks;
using static System.Text.Json.JsonSerializer;
using static CommandEngine.Commands.CommandEnvironment;
using static CommandEngine.Commands.EnvironmentSerializer;

namespace CommandEngine.Commands
{
    internal class EnvironmentDeserializer : CommandVisitor
    {
        private readonly CommandEnvironment _environment;
        private readonly List<CommandState> _states;

        public EnvironmentDeserializer(string json)
        {
            try {
                var dto = Deserialize<ExtensionDto>(json) ?? throw new InvalidOperationException("Invalid Json");
                _states = dto.States;
                _environment = RestoredEnvironment(
                    Environment(new Guid(dto.EnvironmentId)),
                    new Guid(dto.ClientId),
                    new Context());
                _environment.Accept(this);
            }
            catch (InvalidOperationException) {
                throw;
            }
            catch (Exception e) {
                throw new InvalidOperationException($"Error deserializing environment: {e.Message}", e);
            }
        }

        public CommandEnvironment Result => _environment;

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask)
        {
            command.State(_states[0]);
            _states.RemoveAt(0);
        }
    }
}