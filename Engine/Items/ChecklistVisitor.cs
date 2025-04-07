using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items;

// Supports walking a Checklist Item hierarchy
public interface ChecklistVisitor: HistoryVisitor {
    void PreVisit(Checklist checklist, Person creator, History history) {}
    void PostVisit(Checklist checklist, Person creator, History history) {}
    void Visit(NullItem item) { }
    void Visit(BooleanItem item, Guid id, string question, bool? value, Dictionary<Person, List<Operation>> operations, History history) {}
    void Visit(MultipleChoiceItem item, Guid id, string question, object? value, List<object> choices, Dictionary<Person, List<Operation>> operations, History history) {}
    void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem, Dictionary<Person, List<Operation>> operations) {}
    void PostVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem, Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(NotItem item, Item negatedItem, Dictionary<Person, List<Operation>> operations) {}
    void PostVisit(NotItem item, Item negatedItem, Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(OrItem item, Item item1, Item item2, Dictionary<Person, List<Operation>> operations) {}
    void PostVisit(OrItem item, Item item1, Item item2, Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(GroupItem item, List<Item> childItems, Dictionary<Person, List<Operation>> operations) { }
    void PostVisit(GroupItem item, List<Item> childItems, Dictionary<Person, List<Operation>> operations) { }
}