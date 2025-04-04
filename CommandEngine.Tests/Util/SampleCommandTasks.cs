using CommandEngine.Commands;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Util.SuspendLabels;
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()


namespace CommandEngine.Tests.Util
{

    internal class PermanentStatus(CommandStatus status) : CommandTask,MementoTask
    {
        internal static readonly PermanentStatus AlwaysSuccessful = new(Succeeded);
        internal static readonly PermanentStatus AlwaysFail = new(Failed);
        internal static readonly PermanentStatus AlwaysSuspended = new(Suspended);
        private readonly CommandStatus status = status;

        public CommandStatus Execute(Context c) => status;
        
        public List<Enum> NeededLabels => [];
        
        public List<Enum> ChangedLabels => [];
        
        public override string ToString() => $"Task always is {status} ";
        
        public CommandTask Clone() => this;
        
        public override bool Equals(object? obj) => 
            this == obj || obj is PermanentStatus other &&  this.status == other.status;
        
        public override int GetHashCode() => status.GetHashCode();
        
        public string ToMemento() => status.ToString();
        
        public static PermanentStatus FromMemento(string memento) => memento switch
        {
            "Succeeded" => AlwaysSuccessful,
            "Failed" => AlwaysFail,
            "Suspended" => AlwaysSuspended,
            _ => throw new InvalidOperationException()
        };
    }

    internal class CrashingTask : CommandTask
    {
        public List<Enum> NeededLabels => [];

        public List<Enum> ChangedLabels => [];
        
        public override string ToString() => $"Task always Crashes ";

        public CommandStatus Execute(Context c) => throw new InvalidOperationException("unable to execute this task");
    }

    internal class RunOnceTask(bool hasRun = false) : CommandTask , MementoTask
    {
        private bool _hasRun = hasRun;
        public List<Enum> NeededLabels => [];
        public List<Enum> ChangedLabels => [];
        public override string ToString() => $"Task only run once";
        
        public CommandStatus Execute(Context c)
        {
            if (_hasRun) throw new InvalidOperationException("unable to execute this task twice");
            _hasRun = true;
            return Succeeded;
        }
        
        public CommandTask Clone() => new RunOnceTask(_hasRun);
        
        public override bool Equals(object? obj) =>
            this == obj || obj is RunOnceTask other && this._hasRun == other._hasRun;

        public string? ToMemento() => _hasRun.ToString();
        
        public static RunOnceTask FromMemento(string memento) => new (memento == "true");
    }
    
    internal class SuspendFirstOnly : CommandTask
    {
        public List<Enum> NeededLabels => [HasRun];

        public List<Enum> ChangedLabels => [HasRun];
        public override string ToString() => $"Task Suspends on first Execution ";
        public CommandStatus Execute(Context c)
        {
            if (c.Has(HasRun)) return Succeeded;
            c[HasRun] = true;
            return Suspended;
        }
       
    }
    internal class CountingTask(int count = 0) : CommandTask , MementoTask
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

        public override bool Equals(object? obj) =>
            this == obj || obj is CountingTask other && this.Equals(other);

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => _count.GetHashCode();

        private bool Equals(CountingTask other) => this._count == other._count;

        public string ToMemento() => _count.ToString(); // Invoked via reflection
        
        public static CountingTask FromMemento(string memento) => new(int.Parse(memento));  // Invoked via reflection

        public CommandTask Clone() => new CountingTask(_count); // Invoked via reflection
    }
    
    internal class ContextTask(List<Enum> neededLabels, List<Enum> changedLabels, List<Enum> missingLabels) : CommandTask, MementoTask
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
        
        public CommandTask Clone() => this; // Invoked via reflection

        public override bool Equals(object? obj) => base.Equals(obj);
        public string? ToMemento() => null;
        public static ContextTask FromMemento(string memento) => throw new NotImplementedException();
       
    }

    internal class WriteTask(List<Enum> writtenLabels) : CommandTask,MementoTask
    {
        private readonly List<Enum> _writtenLabels = writtenLabels;

        public List<Enum> NeededLabels => [];

        public List<Enum> ChangedLabels => [];
        public override string ToString() => $"Task Writes labels {string.Join(", ", _writtenLabels)}" ;

        public CommandStatus Execute(Context c)
        {
            foreach (var label in _writtenLabels) c[label] = (label.ToString() ?? "null").ToUpper() + "Changed";
            return Succeeded;
        }
        public CommandTask Clone() => this;
        public override bool Equals(object? obj) => base.Equals(obj);
        public string? ToMemento() => null;
        public static WriteTask FromMemento(string memento) => throw new NotImplementedException();
    }
    
    internal class ReadTask(List<Enum> neededLabels) : CommandTask, MementoTask
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
        public CommandTask Clone() => this;
        public override bool Equals(object? obj) => base.Equals(obj);
        public string? ToMemento() => null;
        public static ReadTask FromMemento(string memento) => throw new NotImplementedException();
    }

    internal enum SuspendLabels
    {
        HasRun,
        CountingTaskCount
    }
}