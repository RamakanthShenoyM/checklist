
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
            _root = new SerialCommandDto(name);
        }

        public interface CommandDto
        {
        }

        public class SerialCommandDto : CommandDto
        {
            private string _name;
            public List<CommandDto> _subCommands = [];

            public string Name { get => _name; set => _name = value; }

            public SerialCommandDto(string name)
            {
                _name = name;
            }

            internal SerialCommand ToCommand() => _name.Sequence(Ignore.Otherwise(Ignore));
        }
    }
}
