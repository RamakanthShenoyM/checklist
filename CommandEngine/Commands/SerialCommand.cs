using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands {
    public class SerialCommand : Command {
        private readonly List<Command> _commands;

        public SerialCommand(Command firstCommand, params Command[] commands) {
            _commands = commands.ToList();
            _commands.Insert(0, firstCommand);
        }

        public SerialCommand(List<Command> commands)
            : this(commands[0], commands.GetRange(1, commands.Count() - 1).ToArray()) { }

        public static SerialCommand Sequence(Command firstCommand, params Command[] commands) =>
            new SerialCommand(firstCommand, commands);

        public Command this[int index] => _commands[index];

        public void Accept(CommandVisitor visitor) {
            visitor.PreVisit(this);
            foreach (var command in _commands) command.Accept(visitor);
            visitor.PostVisit(this);
        }

        public CommandStatus Execute(Context c) {
            var status = _commands[0].Execute(c);
            switch (status) {
                case Succeeded:
                    if (_commands.Count == 1) return Succeeded;
                    var restStatus = new SerialCommand(_commands.GetRange(1, _commands.Count - 1)).Execute(c);
                    switch (restStatus) {
                        case Succeeded:
                            return Succeeded;
                        case Failed:
                        case Reverted:
                            return _commands[0].Undo(c);
                    }

                    break;
                case Reverted:
                    return Reverted;
                case Failed:
                    return Failed;
                default:
                    throw new InvalidOperationException("Unexpected result from execution");
            }

            throw new InvalidOperationException("Unexpected result from execution");
        }

        public CommandStatus Undo(Context c) {
            foreach (var command in _commands.AsEnumerable().Reverse().ToList()) command.Undo(c);
            return Reverted;
        }
    }
}