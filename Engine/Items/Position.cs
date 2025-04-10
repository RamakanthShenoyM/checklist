using System.Text.RegularExpressions;

namespace Engine.Items;

public class Position {
    private readonly List<int> _indexes;

    private Position(List<int> indexes) {
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

    internal List<int> ToIndexes()=> [.._indexes];
}