using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands
{
    public class SimpleCommand
    {
        private readonly CommandTask _task;
        private readonly CommandTask _revertTask;

        public SimpleCommand(CommandTask task, CommandTask revertTask)
        {
            _task = task;
            _revertTask = revertTask;
        }

        public CommandStatus Execute()
        {
            try
            {
                var status = _task.Execute();
                if (status == Suspended) throw new TaskSuspendedException(_task, this);
                return status;
            }
            catch (TaskSuspendedException)
            {
                throw;
            }
            catch (Exception)
            {
                return Failed;
            }
        }

        public CommandStatus Undo()
        {
            try
            {
                if (_revertTask.Execute() == Failed) throw new UndoTaskFailureException(_revertTask, this);
                if (_revertTask.Execute() == Suspended) throw new TaskSuspendedException(_revertTask, this);           
                return Reverted;
            }
            catch (TaskSuspendedException)
            {
                throw;
            }
            catch (UndoTaskFailureException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new UndoTaskFailureException(_revertTask, this);
            }
          
        }
    }
}
