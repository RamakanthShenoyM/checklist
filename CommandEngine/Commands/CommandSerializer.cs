
using CommandEngine.Tasks;
using static CommandEngine.Tasks.CommandTask;

namespace CommandEngine.Commands
{
    internal class CommandSerializer : CommandVisitor
    {
        private SerialCommandDto _root;
        public CommandSerializer(SerialCommand command)
        {
            command.Accept(this);
        }

        internal string Result => System.Text.Json.JsonSerializer.Serialize(_root);

        public void PreVisit(SerialCommand command, string name, List<Command> subCommands)
        {
            _root = new SerialCommandDto(name, new List<SimpleCommandDto>());
        }

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask)
        {
            _root.SubCommands.Add(new SimpleCommandDto(state, executeTask.GetType().ToString(), revertTask.GetType().ToString()));
        }

        public interface CommandDto
        {
        }

        public class SerialCommandDto(string name, List<SimpleCommandDto> subCommands) : CommandDto
        {
            public string Name { get => name; set => name = value; }
            public List<SimpleCommandDto> SubCommands { get => subCommands; set => subCommands = value; }

            internal SerialCommand ToCommand() => name.Sequence(Ignore.Otherwise(Ignore));
        }

        public class SimpleCommandDto : CommandDto
        {
            public SimpleCommandDto(CommandState state, string executeTaskClass, string revertTaskClass)
            {
                State = state;
                ExecuteTaskClass = executeTaskClass;
                RevertTaskClass = revertTaskClass;
            }

            public CommandState State { get; }
            public string ExecuteTaskClass { get; }
            public string RevertTaskClass { get; }
        }
    }
}
