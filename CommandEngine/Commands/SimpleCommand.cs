using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands
{

	public class SimpleCommand(CommandTask executeTask, CommandTask revertTask) : Command
	{
		private SimpleCommandState _state = new Initial();

		public void Accept(CommandVisitor visitor) => 
			visitor.Visit(this, _state.State(), executeTask, revertTask);

		public CommandStatus Execute(Context c) => _state.Execute(this, c);

		public CommandStatus Undo(Context c) => _state.Undo(this, c);
		
		private void State(SimpleCommandState newState, Context c)
        {
			c.Event(this, _state.State(), newState.State());
			_state = newState;
		}
		
        public override string ToString() => $"Command with Task <{executeTask}> and revert Task <{revertTask}>";
        
        private CommandStatus RealExecute(Context c)
		{
            var subContext = c.SubContext(executeTask.NeededLabels);
			c.History.Event(this, executeTask);
            try
			{
				var status = executeTask.Execute(subContext);
				if (status == Suspended) throw new TaskSuspendedException(executeTask, this);
                Update(executeTask, c, subContext);
                return status;
			}
			catch (ConclusionException)
			{
                Update(executeTask, c, subContext);
                State(new Executed(), c);
				throw;
			}
			catch (TaskSuspendedException)
			{
                Update(executeTask, c, subContext);
                throw;
			}
			catch (Exception)
			{
				return Failed;
			}
		}

        private void Update(CommandTask task, Context c, Context subContext)
        {
            Dictionary<object, object?> previousValues = task.ChangedLabels.ToDictionary(label => label, label=> c.Has(label)?c[label]:null);
			c.Update(subContext, task.ChangedLabels);
            foreach (var keyValuePair in previousValues)
            {
				if (keyValuePair.Value != (c.Has(keyValuePair.Key) ? c[keyValuePair.Key] : null))
				 c.Event(this,task, keyValuePair.Key, keyValuePair.Value, c.Has(keyValuePair.Key) ? c[keyValuePair.Key] : null);
            }
        }

        private CommandStatus RealUndo(Context c)
		{
            var subContext = c.SubContext(revertTask.NeededLabels);
            try
			{
				var status = revertTask.Execute(subContext);
                if (status == Failed)
				{
					State(new FailedToUndo(), c);
					throw new UndoTaskFailureException(revertTask, this);
				}
                Update(revertTask, c, subContext);
                if (status == Suspended) throw new TaskSuspendedException(revertTask, this);
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
			catch (ConclusionException)
			{
				Update(revertTask, c, subContext);
				throw;
			}
			catch (Exception)
			{	
				State(new FailedToUndo(), c);
				throw new UndoTaskFailureException(revertTask, this);
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