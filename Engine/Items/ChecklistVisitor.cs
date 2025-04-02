using Engine.Persons;

namespace Engine.Items;

// Supports walking a Checklist Item hierarchy
public interface ChecklistVisitor {
    void PreVisit(Checklist checklist, Person creator) {}
    void PostVisit(Checklist checklist, Person creator) {}
    void Visit(BooleanItem item, Guid id, string question, bool? value, Dictionary<Person, List<Operation>> operations) {}
    void Visit(MultipleChoiceItem item, Guid id, string question, object? value, List<object> choices,
        Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {}
    void PostVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {}
    void PreVisit(NotItem item, Item negatedItem) {}
    void PostVisit(NotItem item, Item negatedItem) {}
    void PreVisit(OrItem item, Item item1, Item item2) {}
    void PostVisit(OrItem item, Item item1, Item item2) {}
    void PreVisit(GroupItem item, List<Item> childItems) { }
    void PostVisit(GroupItem item, List<Item> childItems) { }
}