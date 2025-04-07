using Engine.Persons;

namespace Engine.Items;

// Understands DSL shortcuts to create Checklist structures
public static class ChecklistExtensions {
    public static Checklist Checklist(this Person creator, Item firstItem, params Item[] items) =>
        new(creator, firstItem, items);

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

    // From ChatGPT
    public static bool DeepEquals(this Dictionary<Person, List<Operation>> left,
        Dictionary<Person, List<Operation>> right) {
        // First, check if the keys match
        if (!left.Keys.SetEquals(right.Keys)) return false;
        // Then, compare the values ignoring order
        foreach (var key in left.Keys) {
            var leftOperations = left[key];
            var rightOperations = right[key];
            if (leftOperations.Count != rightOperations.Count) return false;

            var leftSet = new HashSet<Operation>(leftOperations);
            var rightSet = new HashSet<Operation>(rightOperations);
            if (!leftSet.SetEquals(rightSet)) return false;
        }

        return true;
    }

    // Helper extension method
    private static bool SetEquals<T>(this ICollection<T> a, ICollection<T> b) =>
        new HashSet<T>(a).SetEquals(b);
}