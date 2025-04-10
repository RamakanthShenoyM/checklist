namespace CommonUtilities.Util;
using static System.String;

public class History
{
    private readonly List<string> _events;
    private const string _wrapperBreak = "========================";

    public History(List<string> events)
    {
        _events = events;
    }

    public override bool Equals(object? obj) =>
        this == obj || obj is History other && this.Equals(other);

    private bool Equals(History other) =>
        this._events.SequenceEqual(other._events);
    public void Update(History other, Enum startEvent, Enum endEvent)
    {
        if (other._events.Count == 0) return;
        _events.Add(Header(startEvent) + _wrapperBreak);
        _events.AddRange(other._events);
        _events.Add(Header(endEvent) + _wrapperBreak);
    }

    public override int GetHashCode() => string.Join("", this._events).GetHashCode();
    public void Accept(HistoryVisitor visitor) => visitor.Visit(this, new List<string>(_events));
    public override string ToString() => Join("\n", _events);

    public List<string> Events(Enum type) =>
        _events.FindAll(e => e.Contains($">> {type} <<"));

    public List<string> Events() => _events;

    public void Add(Enum type, string message) => _events.Add(Header(type) + message);

    public string Header(Enum type) => $"{DateTime.Now} >> {type} << Status: ";

    public void Merge(History other) => _events.InsertRange(0, other._events);

    public History? Clone() => new(_events);
}
