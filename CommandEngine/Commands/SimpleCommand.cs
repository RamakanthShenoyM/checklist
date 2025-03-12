using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands
{
    public class SimpleCommand
    {
        private readonly CommandTask _task;
        private readonly CommandTask _revertTask;
        private State _state = new Initial();

        public SimpleCommand(CommandTask task, CommandTask revertTask)
        {
            _task = task;
            _revertTask = revertTask;
        }

        public CommandStatus Execute()
        {
            return _state.Execute(this);
        }

        private CommandStatus RealExecute()
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

        public CommandStatus Undo() => _state.Undo(this);

        private CommandStatus RealUndo()
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

        private interface State
        {
            public CommandStatus Execute(SimpleCommand command);
            public CommandStatus Undo(SimpleCommand command);
        }

        private class Initial : State
        {
            public CommandStatus Execute(SimpleCommand command)
            {
                var status = command.RealExecute();
                command._state = new Executed();
                return status;
            }
            public CommandStatus Undo(SimpleCommand command)
            {
                throw new InvalidOperationException("Command has not been executed yet.");
            }
        }

        private class Executed : State
        {
            public CommandStatus Execute(SimpleCommand command)
            {
                return Succeeded;
            }
            public CommandStatus Undo(SimpleCommand command) => command.RealUndo();
        }
    }
}
