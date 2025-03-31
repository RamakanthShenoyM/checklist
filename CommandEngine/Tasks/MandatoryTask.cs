using CommandEngine.Commands;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Tasks {
    // Implements the Decorator Pattern from GoF
    public class MandatoryTask(CommandTask subTask) : CommandTask, MementoTask {
        private readonly CommandTask _subTask = subTask;

        public List<Enum> NeededLabels => _subTask.NeededLabels;

        public List<Enum> ChangedLabels => _subTask.ChangedLabels;

        public CommandStatus Execute(Context c) =>
            NeededLabels.All(c.Has) ? _subTask.Execute(c) : Suspended;

        public CommandTask Clone() {
            if (!_subTask.GetType().NeedsMemento()) return this;
            var type = _subTask.GetType();
            var method = type.InstanceMethod("Clone") ?? throw new InvalidOperationException("Clone method is missing for wrapped subtask");
                #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            var newSubtask = (CommandTask)method.Invoke(type, parameters: null) ?? throw new InvalidOperationException("Failure in cloning subtask; possible missing method");
                #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            return new MandatoryTask(newSubtask);
        }

        public override bool Equals(object? obj) =>
            this == obj || obj is MandatoryTask other && this._subTask.Equals(other._subTask);
        
        public override int GetHashCode() => _subTask.GetHashCode();

        public string? ToMemento() {
            if (!_subTask.GetType().NeedsMemento()) return null;
            var type = _subTask.GetType();
            var method = type.InstanceMethod("ToMemento");
            return (string?)method?.Invoke(type, null);
        }

        public static MandatoryTask FromMemento(string memento) =>
            throw new InvalidOperationException("MandatoryTask shouldn't need to be restored from a memento");
    }
}