using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands {
    
    public class SimpleCommand(CommandTask task, CommandTask revertTask) : Command {
        private State _state = new Initial();

        public void Accept(CommandVisitor visitor)
        {
            visitor.Visit(this, _state.State());
        }

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
            public CommandState State();
        }

        private class Initial : State {
            public CommandStatus Execute(SimpleCommand command) {
                var status = command.RealExecute();
                command._state = new Executed();
                return status;
            }

            public CommandState State() => CommandState.Initial;

            public CommandStatus Undo(SimpleCommand command) =>
                throw new InvalidOperationException("Command has not been executed yet.");
        }

        private class Executed : State {
            public CommandState State() => CommandState.Executed;
            public CommandStatus Execute(SimpleCommand command) => Succeeded;
            public CommandStatus Undo(SimpleCommand command)
            {
                var status = command.RealUndo();
                command._state = new Reversed();
                return status;
            }
        }

        private class Reversed : State
        {
            public CommandState State() => CommandState.Reversed;
            public CommandStatus Execute(SimpleCommand command) => 
                throw new InvalidOperationException("Command has already been executed");

            public CommandStatus Undo(SimpleCommand command) => 
                throw new InvalidOperationException("Command has already been undone");
        }
    }

    public enum CommandState
    {
        Initial,
        Executed,
        Reversed
    }
}