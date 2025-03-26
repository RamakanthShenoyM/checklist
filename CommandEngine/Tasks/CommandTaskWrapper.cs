using CommandEngine.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommandEngine.Commands.CommandEnvironment;

namespace CommandEngine.Tasks
{
	public class CommandTaskWrapper : CommandTask, MementoTask
	{
		private readonly CommandEnvironment _environment;

		public CommandTaskWrapper(CommandEnvironment environment)
		{
			_environment = environment;
		}

		public List<Enum> NeededLabels => throw new NotImplementedException();

		public List<Enum> ChangedLabels => throw new NotImplementedException();

		public CommandTask Clone() => new CommandTaskWrapper(FreshEnvironment(_environment));

		public CommandStatus Execute(Context c)
		{
			return _environment.Execute(c);
		}

		string MementoTask.ToMemento()
		{
			throw new NotImplementedException();
		}
	}
}
