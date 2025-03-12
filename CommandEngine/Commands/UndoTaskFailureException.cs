using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandEngine.Commands
{
    public class UndoTaskFailureException: Exception
    {
        private readonly CommandTask _undoTask;
        private readonly SimpleCommand _command;

        public UndoTaskFailureException(CommandTask undoTask,SimpleCommand command) : base("Undo Failure")
        {
            _undoTask = undoTask;
            _command = command;
        }
    }
}
