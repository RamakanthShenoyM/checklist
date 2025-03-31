using Engine.Persons;

namespace Engine.Items;

// Understands DSL shortcuts to create Checklist structures
public static class ChecklistExtensions {
    public static Checklist Checklist(this Person creator, Item firstItem, params Item[] items) =>
        new (creator, firstItem, items);
    public static GroupItem Group(Item firstItem, params Item[] items) =>
        new (firstItem, items);

    public static OrItem Or(Item item1, Item item2) =>
        new (item1, item2);
    public static NotItem Not(Item item) =>
        new (item);
    public static ConditionalItem Conditional(Item condition, Item? onSuccess = null, Item? onFailure = null) =>
        new (condition, onSuccess, onFailure);

    public static List<Position> Positions(this Checklist checklist, Item item) =>
        new PositionLocator(checklist, item).Results;
}