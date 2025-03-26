using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public class ConclusionTask(object conclusion) : CommandTask
    {
        public List<Enum> NeededLabels => new();

        public List<Enum> ChangedLabels => new();

        public CommandStatus Execute(Context c)
        {
            throw new ConclusionException(conclusion);
        }
        public ConclusionTask Clone() => this;
        public override bool Equals(object? obj) => base.Equals(obj);
        public string? ToMemento() => null;
        public static ConclusionTask FromMemento(string memento) => throw new NotImplementedException();
    }
}

