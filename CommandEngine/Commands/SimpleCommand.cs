using CommandEngine.Tasks;
using System.Windows.Input;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands
{

    public class SimpleCommand : Command
    {
        private SimpleCommandState _state = new Initial();
        private readonly CommandTask _executeTask;
        private readonly CommandTask _revertTask;

        internal SimpleCommand(CommandTask executeTask, CommandTask revertTask)
        {
            _executeTask = executeTask;
            _revertTask = revertTask;
        }

        public void Accept(CommandVisitor visitor) =>
            visitor.Visit(this, _state.State(), _executeTask, _revertTask);

        public CommandStatus Execute(Context c) => _state.Execute(this, c);

        public CommandStatus Undo(Context c) => _state.Undo(this, c);
        public Command Clone() => new SimpleCommand(_executeTask, _revertTask);
        public override bool Equals(object? obj) =>
          this == obj || obj is SimpleCommand other && this.Equals(other);

        public override int GetHashCode() => _executeTask.GetHashCode() * 37 + _revertTask.GetHashCode();
        private bool Equals(SimpleCommand other) => 
            this._executeTask.Equals(other._executeTask) 
            && this._revertTask.Equals(other._revertTask)
            && this._state.State() == other._state.State();
        private void State(SimpleCommandState newState, Context c)
        {
            c.Event(this, _state.State(), newState.State());
            _state = newState;
        }

        internal void State(CommandState state)
        {
            _state = state switch
            {
                CommandState.Initial => new Initial(),
                CommandState.Executed => new Executed(),
                CommandState.Reversed => new Reversed(),
                CommandState.FailedToExecute => new FailedToExecute(),
                CommandState.FailedToUndo => new FailedToUndo(),
                _ => throw new System.InvalidOperationException("Invalid CommandState")
            };
        }

        public override string ToString() =>
            (_revertTask is IgnoreTask) ?
             $"Command with Task <{_executeTask}> without Undo"
            : $"Command with Task <{_executeTask}> and revert Task <{_revertTask}>";

        private CommandStatus RealExecute(Context c)
        {
            var subContext = c.SubContext(_executeTask.NeededLabels);
            c.History.Event(this, _executeTask);
            try
            {
                var status = _executeTask.Execute(subContext);
                c.Event(this, _executeTask, status);
                if (status == Suspended) throw new TaskSuspendedException(_executeTask, this);
                if (status == Succeeded) Update(_executeTask, c, subContext);
                return status;
            }
            catch (ConclusionException e)
            {
                Update(_executeTask, c, subContext);
                c.Event(this, _executeTask, e.Conclusion);
                State(new Executed(), c);
                throw;
            }
            catch (TaskSuspendedException)
            {
                Update(_executeTask, c, subContext);
                throw;
            }
            catch(MissingContextInformationException e) 
            {
                c.Event(this, _executeTask, e, e.MissingLabel);
                return Failed;
            }
            catch (Exception e)
            {
                c.Event(this, _executeTask, e);
                return Failed;
            }
        }

        private void Update(CommandTask task, Context c, Context subContext)
        {
            Dictionary<object, object?> previousValues = task.ChangedLabels.ToDictionary(label => label, label => c.Has(label) ? c[label] : null);
            c.Update(subContext, task.ChangedLabels);
            foreach (var keyValuePair in previousValues)
            {
                if (keyValuePair.Value != (c.Has(keyValuePair.Key) ? c[keyValuePair.Key] : null))
                    c.Event(this, task, keyValuePair.Key, keyValuePair.Value, c.Has(keyValuePair.Key) ? c[keyValuePair.Key] : null);
            }
        }

        private CommandStatus RealUndo(Context c)
        {
            var subContext = c.SubContext(_revertTask.NeededLabels);
            try
            {
                var status = _revertTask.Execute(subContext);
                c.Event(this, _revertTask, status);
                if (status == Failed)
                {
                    State(new FailedToUndo(), c);
                    throw new UndoTaskFailureException(_revertTask, this);
                }
                Update(_revertTask, c, subContext);
                if (status == Suspended) throw new TaskSuspendedException(_revertTask, this);
                return Reverted;
            }
            catch (UndoTaskFailureException)
            {
                throw;
            }
            catch (TaskSuspendedException)
            {
                throw;
            }
            catch (ConclusionException e)
            {
                Update(_revertTask, c, subContext);
                c.Event(this, _revertTask, e.Conclusion);
                throw;
            }
            catch (Exception e)
            {
                c.Event(this, _revertTask, e);
                State(new FailedToUndo(), c);
                throw new UndoTaskFailureException(_revertTask, this);
            }
        }

        private interface SimpleCommandState
        {
            public CommandStatus Execute(SimpleCommand command, Context c);
            public CommandStatus Undo(SimpleCommand command, Context c);
            public CommandState State();
        }

        private class Initial : SimpleCommandState
        {
            public CommandStatus Execute(SimpleCommand command, Context c)
            {
                var status = command.RealExecute(c);

                command.State(status == Succeeded ? new Executed() : new FailedToExecute(), c);
                return status;
            }

            public CommandState State() => CommandState.Initial;

            public CommandStatus Undo(SimpleCommand command, Context c) =>
                    throw new InvalidOperationException("Command has not been executed yet.");
        }

        private class Executed : SimpleCommandState
        {
            public CommandState State() => CommandState.Executed;
            public CommandStatus Execute(SimpleCommand command, Context c) => Succeeded;
            public CommandStatus Undo(SimpleCommand command, Context c)
            {
                var status = command.RealUndo(c);
                command.State(new Reversed(), c);
                return status;
            }
        }

        private class Reversed : SimpleCommandState
        {
            public CommandState State() => CommandState.Reversed;
            public CommandStatus Execute(SimpleCommand command, Context c) =>
                    throw new InvalidOperationException("Command has already been executed");

            public CommandStatus Undo(SimpleCommand command, Context c) =>
                    throw new InvalidOperationException("Command has already been undone");
        }

        private class FailedToExecute : SimpleCommandState
        {
            public CommandState State() => CommandState.FailedToExecute;
            public CommandStatus Execute(SimpleCommand command, Context c) =>
                    throw new InvalidOperationException("Command has already failed");

            public CommandStatus Undo(SimpleCommand command, Context c) =>
                    throw new InvalidOperationException("Command has already failed");
        }
        private class FailedToUndo : SimpleCommandState
        {
            public CommandState State() => CommandState.FailedToUndo;
            public CommandStatus Execute(SimpleCommand command, Context c) =>
                    throw new InvalidOperationException("Command undo already failed");

            public CommandStatus Undo(SimpleCommand command, Context c) =>
                    throw new InvalidOperationException("Command undo already failed");
        }
    }

    public enum CommandState
    {
        Initial,
        Executed,
        Reversed,
        FailedToExecute,
        FailedToUndo
    }
}