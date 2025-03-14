using CommandEngine.Commands;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;


namespace CommandEngine.Tests.Util
{

    internal class PermanentStatus(CommandStatus status) : CommandTask
    {
        internal static readonly PermanentStatus AlwaysSuccessful = new(Succeeded);
        internal static readonly PermanentStatus AlwaysFail = new(Failed);
        internal static readonly PermanentStatus AlwaysSuspended = new(Suspended);
        public CommandStatus Execute(Context c) => status;
        public List<object> NeededLabels => new();
        public List<object> ChangedLabels => new();
    }

    internal class CrashingTask : CommandTask
    {
        public List<object> NeededLabels => new();

        public List<object> ChangedLabels => new();

        public CommandStatus Execute(Context c) => throw new InvalidOperationException("unable to execute this task");
    }

    internal class RunOnceTask : CommandTask
    {
        private bool _hasRun;
        public List<object> NeededLabels => new();

        public List<object> ChangedLabels => new();
        public CommandStatus Execute(Context c)
        {
            if (_hasRun) throw new InvalidOperationException("unable to execute this task twice");
            _hasRun = true;
            return Succeeded;
        }
    }

    internal class SuspendFirstOnly : CommandTask
    {
        private bool _hasSuspended;
        public List<object> NeededLabels => new();

        public List<object> ChangedLabels => new();
        public CommandStatus Execute(Context c)
        {
            if (_hasSuspended) return Succeeded;
            _hasSuspended = true;
            return Suspended;
        }
    }
    internal class ContextTask(List<object> neededLabels, List<object> changedLabels, List<object> missingLabels) : CommandTask
    {
        public List<object> NeededLabels => neededLabels;

        public List<object> ChangedLabels => changedLabels;

        public CommandStatus Execute(Context c)
        {
            foreach (var label in missingLabels) Assert.False(c.Has(label), $"Unexpected label {label}");
            foreach (var label in changedLabels) c[label] = (label.ToString() ?? "null").ToUpper() + "Changed";
            return Succeeded;
        }
    }
}