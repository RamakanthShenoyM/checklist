using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public class ConclusionTask(object conclusion) : CommandTask, MementoTask
    {
        private readonly object _conclusion = conclusion;
        
        public List<Enum> NeededLabels => new();

        public List<Enum> ChangedLabels => new();

        public CommandStatus Execute(Context c) => throw new ConclusionException(_conclusion);
        
        public CommandTask Clone() => this;
        
        public override bool Equals(object? obj) => 
            this==obj || obj is ConclusionTask other && this._conclusion == other._conclusion;
        
        public override int GetHashCode() => _conclusion.GetHashCode();
        
        public string? ToMemento() => null;
        
        public static ConclusionTask FromMemento(string memento) => 
            throw new InvalidOperationException("ConclusionTask shouldn't need to be restored from a memento");
    }
}

