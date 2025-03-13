using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands
{
    public class SerialCommand : Command
    {
        private readonly List<Command> _commands;

        public SerialCommand(Command firstCommand, params Command[] commands)
        {
            _commands = commands.ToList();
            _commands.Insert(0, firstCommand);
        }

        private SerialCommand(List<Command> commands)
        {
            _commands = commands;
        }

        public void Accept(CommandVisitor visitor)
        {
            visitor.PreVisit(this);
            foreach (var command in _commands) command.Accept(visitor);
            visitor.PostVisit(this);
        }

        public CommandStatus Execute()
        {
            var status = _commands[0].Execute();
            switch (status)
            {
                case Succeeded:
                    if (_commands.Count == 1) return Succeeded;
                    var restStatus = new SerialCommand(_commands.GetRange(1, _commands.Count - 1)).Execute();
                    switch (restStatus)
                    {
                        case Succeeded:
                            return Succeeded;
                        case Failed:
                        case Reverted:
                            return _commands[0].Undo();
                    }
                    break;
                case Failed:
                    return Failed;
                default:
                    throw new InvalidOperationException("Unexpected result from execution");
            }
            throw new InvalidOperationException("Unexpected result from execution");
        }

        public CommandStatus Undo()
        {
            throw new NotImplementedException();
        }
    }
}
