using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public CommandStatus Execute()
        {
            foreach (var command in _commands)
            {
                if (command.Execute() == Failed)
                {
                    return Failed;
                }
            }
            return Succeeded;
        }

        public CommandStatus Undo()
        {
            throw new NotImplementedException();
        }
    }
}
