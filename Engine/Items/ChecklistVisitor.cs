using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items;

// Supports walking a Checklist Item hierarchy
public interface ChecklistVisitor: HistoryVisitor {
    void PreVisit(Checklist checklist, 
        Person creator, 
        History history) {}
    void PostVisit(Checklist checklist, 
        Person creator, 
        History history) {}
    void Visit(NullItem item) { }
    void Visit(BooleanItem item,
        Guid id,
        Position position,
        string question,
        bool? value,
        Dictionary<Person, List<Operation>> operations,
        History history) {}
    void Visit(MultipleChoiceItem item,
        Guid id,
        Position position,
        string question,
        object? value,
        List<object> choices,
        Dictionary<Person, List<Operation>> operations,
        History history) {}
    void PreVisit(ConditionalItem item,
        Position position,
        Item baseItem,
        Item? successItem,
        Item? failureItem,
        Dictionary<Person, List<Operation>> operations) {}
    void PostVisit(ConditionalItem item,
        Position position,
        Item baseItem,
        Item? successItem,
        Item? failureItem,
        Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(NotItem item,
        Position position,
        Item negatedItem,
        Dictionary<Person, List<Operation>> operations) {}
    void PostVisit(NotItem item,
        Position position,
        Item negatedItem,
        Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(OrItem item,
        Position position,
        Item item1,
        Item item2,
        Dictionary<Person, List<Operation>> operations) {}
    void PostVisit(OrItem item,
        Position position,
        Item item1,
        Item item2,
        Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(GroupItem item,
        Position position,
        List<Item> childItems,
        Dictionary<Person, List<Operation>> operations) { }
    void PostVisit(GroupItem item,
        Position position,
        List<Item> childItems,
        Dictionary<Person, List<Operation>> operations) { }
}