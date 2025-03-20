using Engine.Persons;

namespace Engine.Items;

// Understands DSL shortcuts to create Checklist structures
public static class ChecklistExtensions {
    public static Checklist Checklist(this Person creator, Item firstItem, params Item[] items) =>
        new Checklist(creator, firstItem, items);
}