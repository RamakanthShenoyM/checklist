namespace Engine.Items;

public class Position {
    private readonly List<int> _indexes;

    private Position(List<int> indexes) {
        _indexes = indexes;
    }

    public Position() : this([0]) { }

    public override string ToString() => string.Join(".", _indexes);

    public void Deeper() => _indexes.Add(0);

    public void Truncate() => _indexes.RemoveAt(_indexes.Count - 1);

    public void Increment() => _indexes[^1]++;

    internal Position Clone() => new(new List<int>(_indexes));
}