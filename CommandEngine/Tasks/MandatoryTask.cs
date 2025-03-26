using CommandEngine.Commands;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Commands.CommandReflection;

namespace CommandEngine.Tasks
{
    public class MandatoryTask(CommandTask subTask) : CommandTask, MementoTask
    {
        private readonly CommandTask _subTask = subTask;

        public List<Enum> NeededLabels => _subTask.NeededLabels;

        public List<Enum> ChangedLabels => _subTask.ChangedLabels;

        public CommandStatus Execute(Context c) => 
            NeededLabels.All(c.Has) ? _subTask.Execute(c) : Suspended;
        public CommandTask Clone()
        {
            if(!_subTask.GetType().NeedsMemento()) return this;
            var type = _subTask.GetType();
            var method = type.InstanceMethod("Clone");
            var newSubtask = (CommandTask)method?.Invoke(type, null);
            return new MandatoryTask(newSubtask);
        }
        public override bool Equals(object? obj) => 
            this == obj || obj is MandatoryTask other && this._subTask.Equals(other._subTask);
        public string? ToMemento()
        {
            if (!_subTask.GetType().NeedsMemento()) return null;
            var type = _subTask.GetType();
            var method =  type.InstanceMethod("ToMemento");
            return (string?)method?.Invoke(type, null);
        }

        public static MandatoryTask FromMemento(string memento)
        {
            throw new NotImplementedException();
        }
    }
}