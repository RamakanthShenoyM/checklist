using CommandEngine.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommandEngine.Commands.CommandEnvironment;

namespace CommandEngine.Tasks
{
	public class CommandTaskWrapper(CommandEnvironment environment, List<Enum> neededLabels, List<Enum> changedLabels) : CommandTask, MementoTask
	{
		private readonly CommandEnvironment _environment = environment;

		public List<Enum> NeededLabels => neededLabels;

		public List<Enum> ChangedLabels => changedLabels;

		public CommandTask Clone() => new CommandTaskWrapper(FreshEnvironment(_environment), NeededLabels,ChangedLabels);

		public CommandStatus Execute(Context c) => _environment.Execute(c);

		public string? ToMemento() => throw new NotImplementedException();

		public static CommandTask FromMemento(string memento) => throw new NotImplementedException();

		public override bool Equals(object? obj) => 
			this == obj || obj is CommandTaskWrapper other && this.Equals(other);
		
		private bool Equals(CommandTaskWrapper other) => 
			this.NeededLabels.SequenceEqual(other.NeededLabels) && 
			this.ChangedLabels.SequenceEqual(other.ChangedLabels) &&
			this._environment.Equals(other._environment);
	}
}
