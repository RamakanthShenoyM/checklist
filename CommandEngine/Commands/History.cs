namespace CommandEngine.Commands;
using static System.String;
public class History
{
    private readonly List<string> _events;
    private const string _wrapperBreak = "========================";

    internal History(List<string> events)
    {
        _events = events;
    }
    public override bool Equals(object? obj) =>
        this == obj || obj is History other && this.Equals(other);

    private bool Equals(History other) =>
        this._events.SequenceEqual(other._events);
    internal void Update(History other, Enum startEvent, Enum endEvent)
    {
        if (other._events.Count == 0) return;
        _events.Add(Header(startEvent) + _wrapperBreak);
        _events.AddRange(other._events);
        _events.Add(Header(endEvent) + _wrapperBreak);
    }

    public override int GetHashCode() => string.Join("", this._events).GetHashCode();

    public override string ToString() => Join("\n", _events);

    internal void Accept(CommandVisitor visitor) => visitor.Visit(this, _events);

    public List<string> Events(CommandEventType type) =>
        _events.FindAll(e => e.Contains($">> {type} <<"));

    public void Add(Enum type, string message) => _events.Add(Header(type) + message);

    public string Header(Enum type) => $"{DateTime.Now} >> {type} << Status: ";

    public void Merge(History other) => _events.InsertRange(0, other._events);
    
}
public enum CommandEventType
{
    StartSubTaskHistory,
    EndSubTaskHistory,
    CommandStateChange,
    TaskExecuted,
    ValueChanged,
    GroupSerialStart,
    GroupSerialComplete,
    TaskException,
    TaskStatus,
    ConclusionReached,
    InvalidAccessAttempt,
    UpdateNotCaptured,
    OutSideLabels,
    WrittenLabels,
    SetAndUsedLabels,
    NeededLabelBeforeSet
}