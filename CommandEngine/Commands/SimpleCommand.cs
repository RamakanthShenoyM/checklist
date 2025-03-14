using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Commands
{

	public class SimpleCommand(CommandTask task, CommandTask revertTask) : Command
	{
		private SimpleCommandState _state = new Initial();

		public void Accept(CommandVisitor visitor)
		{
			visitor.Visit(this, _state.State());
		}

		public CommandStatus Execute(Context c) => _state.Execute(this, c);

		public CommandStatus Undo(Context c) => _state.Undo(this, c);

		private CommandStatus RealExecute(Context c)
		{
            var subContext = c.SubContext(task.NeededLabels);
            try
			{
				var status = task.Execute(subContext);
				c.Update(subContext, task.ChangedLabels);
				if (status == Suspended) throw new TaskSuspendedException(task, this);
				return status;
			}
			catch (ConclusionException)
			{
                c.Update(subContext, task.ChangedLabels);
                _state = new Executed();
				throw;
			}
			catch (CommandException)
			{
                c.Update(subContext, task.ChangedLabels);
                throw;
			}
			catch (Exception)
			{
				return Failed;
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
					_state = new FailedToUndo();
					throw new UndoTaskFailureException(revertTask, this);
				}
				c.Update(subContext, revertTask.ChangedLabels);
				if (status == Suspended) throw new TaskSuspendedException(revertTask, this);
				return Reverted;
			}
			catch (CommandException)
			{
				c.Update(subContext, revertTask.ChangedLabels);
				throw;
			}
			catch (Exception)
			{
				_state = new FailedToUndo();
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

				command._state = status == Succeeded ? new Executed() : new FailedToExecute();
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
				command._state = new Reversed();
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