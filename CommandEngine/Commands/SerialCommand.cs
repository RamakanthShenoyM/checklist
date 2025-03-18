using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands {
    public class SerialCommand : Command {
        private readonly List<Command> _commands;
        private readonly string _groupName;

        public SerialCommand(string groupName, Command firstCommand, params Command[] commands)
        {
            _commands = commands.ToList();
            _commands.Insert(0, firstCommand);
            _groupName = groupName;
        }

        public SerialCommand(List<Command> commands)
            : this("first group", commands[0], commands.GetRange(1, commands.Count() - 1).ToArray()) { }

        internal string NameOnly() => _groupName;

        public override string ToString() => new PrettyPrint(this).Result;

        public Command this[int index] => _commands[index];

        public void Accept(CommandVisitor visitor) {
            visitor.PreVisit(this, _groupName, _commands);
            foreach (var command in _commands) command.Accept(visitor);
            visitor.PostVisit(this, _groupName, _commands);
        }

        public CommandStatus Execute(Context c)
        {
            c.StartEvent(this);
            var result = RealExecute(c);
            c.CompletedEvent(this);
            return result;
        }

        private CommandStatus RealExecute(Context c) {
            var status = _commands[0].Execute(c);
            switch (status) {
                case Succeeded:
                    if (_commands.Count == 1) return Succeeded;
                    var restStatus = new SerialCommand(_commands.GetRange(1, _commands.Count - 1)).RealExecute(c);
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