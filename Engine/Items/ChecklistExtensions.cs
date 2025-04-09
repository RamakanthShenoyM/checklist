using Engine.Persons;

namespace Engine.Items;

// Understands DSL shortcuts to create Checklist structures
public static class ChecklistExtensions {
    public static Checklist Checklist(this Person creator, Item firstItem, params Item[] items) =>
        new (creator, firstItem, items:items);
    public static GroupItem Group(Item firstItem, params Item[] items) =>
        new(firstItem, items);

    public static OrItem Or(Item item1, Item item2) =>
        new(item1, item2);

    public static NotItem Not(Item item) =>
        new(item);

    public static ConditionalItem Conditional(Item condition, Item? onSuccess = null, Item? onFailure = null) =>
        new(condition, onSuccess, onFailure);

    public static List<Position> Positions(this Checklist checklist, Item item) =>
        new PositionLocator(checklist, item).Results;

    internal static List<int> ToPosition(this string position)
    {
        var raw=position.Substring(2, position.Length - 3);
        return raw.Split(".").Select(int.Parse).ToList();
    }

    // From ChatGPT
    public static bool DeepEquals<TKey, TEnum>(this Dictionary<TKey, List<TEnum>> left,
        Dictionary<TKey, List<TEnum>> right) where TEnum : Enum where TKey : notnull {
        // First, check if the keys match
        if (!new HashSet<TKey>(left.Keys).SetEquals(right.Keys)) return false;
        // Then, compare the values ignoring order
        foreach (var key in left.Keys) {
            var leftOperations = new HashSet<TEnum>(left[key]);
            var rightOperations = new HashSet<TEnum>(right[key]);
            if (leftOperations.Count != rightOperations.Count) return false;
            if (!leftOperations.SetEquals(rightOperations)) return false;
        }

        return true;
    }
}