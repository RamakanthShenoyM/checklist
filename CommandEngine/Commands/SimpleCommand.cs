using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands {
    
    public class SimpleCommand(CommandTask task, CommandTask revertTask) {
        private State _state = new Initial();

        public CommandStatus Execute() => _state.Execute(this);

        public CommandStatus Undo() => _state.Undo(this);

        private CommandStatus RealExecute() {
            try {
                var status = task.Execute();
                if (status == Suspended) throw new TaskSuspendedException(task, this);
                return status;
            }
            catch (TaskSuspendedException) {
                throw;
            }
            catch (Exception) {
                return Failed;
            }
        }

        private CommandStatus RealUndo() {
            try {
                if (revertTask.Execute() == Failed) throw new UndoTaskFailureException(revertTask, this);
                if (revertTask.Execute() == Suspended) throw new TaskSuspendedException(revertTask, this);
                return Reverted;
            }
            catch (TaskSuspendedException) {
                throw;
            }
            catch (UndoTaskFailureException) {
                throw;
            }
            catch (Exception) {
                throw new UndoTaskFailureException(revertTask, this);
            }
        }

        private interface State {
            public CommandStatus Execute(SimpleCommand command);
            public CommandStatus Undo(SimpleCommand command);
        }

        private class Initial : State {
            public CommandStatus Execute(SimpleCommand command) {
                var status = command.RealExecute();
                command._state = new Executed();
                return status;
            }

            public CommandStatus Undo(SimpleCommand command) =>
                throw new InvalidOperationException("Command has not been executed yet.");
        }

        private class Executed : State {
            public CommandStatus Execute(SimpleCommand command) => Succeeded;
            public CommandStatus Undo(SimpleCommand command) => command.RealUndo();
        }
    }
}