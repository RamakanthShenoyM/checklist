using CommandEngine.Commands;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Util.SuspendLabels;


namespace CommandEngine.Tests.Util
{

    internal class PermanentStatus(CommandStatus status) : CommandTask
    {
        internal static readonly PermanentStatus AlwaysSuccessful = new(Succeeded);
        internal static readonly PermanentStatus AlwaysFail = new(Failed);
        internal static readonly PermanentStatus AlwaysSuspended = new(Suspended);
        public CommandStatus Execute(Context c) => status;
        public List<Enum> NeededLabels => new();
        public List<Enum> ChangedLabels => new();
        public override string ToString() => $"Task always is {status} ";
        public PermanentStatus Clone() => this;
    }

    internal class CrashingTask : CommandTask
    {
        public List<Enum> NeededLabels => new();

        public List<Enum> ChangedLabels => new();
        public override string ToString() => $"Task always Crashes ";

        public CommandStatus Execute(Context c) => throw new InvalidOperationException("unable to execute this task");
    }

    internal class RunOnceTask : CommandTask
    {
        private bool _hasRun;
        public List<Enum> NeededLabels => new();

        public List<Enum> ChangedLabels => new();
        public override string ToString() => $"Task only run once";
        public CommandStatus Execute(Context c)
        {
            if (_hasRun) throw new InvalidOperationException("unable to execute this task twice");
            _hasRun = true;
            return Succeeded;
        }
        public RunOnceTask Clone()
        {
            var result = new RunOnceTask();
            result._hasRun = _hasRun;
            return result;
        }
    }
    internal class SuspendFirstOnly : CommandTask
    {
        public List<Enum> NeededLabels => new() { HasRun };

        public List<Enum> ChangedLabels => new() {HasRun};
        public override string ToString() => $"Task Suspends on first Execution ";
        public CommandStatus Execute(Context c)
        {
            if (c.Has(HasRun)) return Succeeded;
            c[HasRun] = true;
            return Suspended;
        }
       
    }
    internal class CountingTask(int count = 0) : CommandTask
    {
        private int _count = count;
        public List<Enum> NeededLabels => new();

        public List<Enum> ChangedLabels => new() { CountingTaskCount };
        public override string ToString() => $"Task increments a counter ";
        public CommandStatus Execute(Context c)
        {
            _count++;
            c[CountingTaskCount] = _count;
            return Succeeded;
        }
        public CountingTask Clone() => new CountingTask(_count);

        public override bool Equals(object? obj) =>
            this == obj || obj is CountingTask other && this.Equals(other);

        private bool Equals(CountingTask other) => this._count == other._count;
    }
    internal enum SuspendLabels
    {
        HasRun,
        CountingTaskCount
    }
    internal class ContextTask(List<Enum> neededLabels, List<Enum> changedLabels, List<Enum> missingLabels) : CommandTask
    {
        public List<Enum> NeededLabels => neededLabels;

        public List<Enum> ChangedLabels => changedLabels;
        public override string ToString() => $"Task needs labels {string.Join(", ", neededLabels)} and sets {string.Join(", ",changedLabels)} ";

        public CommandStatus Execute(Context c)
        {
            foreach (var label in missingLabels) Assert.False(c.Has(label), $"Unexpected label {label}");
            foreach (var label in changedLabels) c[label] = (label.ToString() ?? "null").ToUpper() + "Changed";
            return Succeeded;
        }
        public ContextTask Clone() => this;
    }
    internal class WriteTask(List<Enum> writtenLabels) : CommandTask
    {
        public List<Enum> NeededLabels => [];

        public List<Enum> ChangedLabels => [];
        public override string ToString() => $"Task Writes labels {string.Join(", ", writtenLabels)}" ;

        public CommandStatus Execute(Context c)
        {
            foreach (var label in writtenLabels) c[label] = (label.ToString() ?? "null").ToUpper() + "Changed";
            return Succeeded;
        }
    }
    internal class ReadTask(List<Enum> neededLabels) : CommandTask
    {
        public List<Enum> NeededLabels => neededLabels;

        public List<Enum> ChangedLabels => [];
        public override string ToString() => $"Task needs Reads labels {string.Join(", ", neededLabels)}";

        public CommandStatus Execute(Context c)
        {
            object x;
            foreach (var label in neededLabels) x = c[label];
            return Succeeded;
        }
    }


}