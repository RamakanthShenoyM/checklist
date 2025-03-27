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
    public static ConditionalItem Conditional(Item baseItem, Item? successItem = null, Item? failItem = null) =>
        new (baseItem, successItem, failItem);
}