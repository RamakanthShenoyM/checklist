using System.Text.RegularExpressions;

namespace Engine.Items;

public class Position {
    private readonly List<int> _indexes;

    private Position(List<int> indexes) {
        _indexes = indexes;
    }

    internal Position(int firstIndex, int[] rest) {
        if (firstIndex != 0)
            throw new InvalidOperationException(
                "There is only one item at the root of the Checklist hierarchy, so use index 0.");
        var indexes = rest.ToList();
        indexes.Insert(0, firstIndex);
        _indexes = indexes;
    }

    public Position() : this([0]) { }

    internal Position(string positionRepresentation) {
        var match = Regex.Match(positionRepresentation.Trim().ToUpper(), @"^P\[(\d+(\.\d+)*)\]$"); // From ChatGPT
        if (!match.Success) throw new ArgumentException($"Invalid position format: '{positionRepresentation}'");
        _indexes = match.Groups[1].Value.Split('.').Select(int.Parse).ToList();
    }

    public override string ToString() => $"P[{string.Join(".", _indexes)}]";

    public void Deeper() => _indexes.Add(0);

    public void Truncate() => _indexes.RemoveAt(_indexes.Count - 1);

    public void Increment() => _indexes[^1]++;

    internal Position Clone() => new([.._indexes]);

    internal List<int> ToIndexes() => [.._indexes];

    public bool IsPartialMatch(Position other) {
        if (this._indexes.Count > other._indexes.Count) return false;
        var matchingIndexes = other._indexes.GetRange(0, this._indexes.Count);
        return this._indexes.SequenceEqual(matchingIndexes);
    }

    public override bool Equals(object? obj) =>
        this == obj || obj is Position other && this.Equals(other);

    private bool Equals(Position other) =>
        this._indexes.SequenceEqual(other._indexes);

    public override int GetHashCode() => ToString().GetHashCode();

    public static bool operator ==(Position left, Position right) => left.Equals(right);

    public static bool operator !=(Position left, Position right) => !left.Equals(right);
}