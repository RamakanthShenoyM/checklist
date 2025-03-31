using Engine.Persons;

namespace Engine.Items;

// Understands the location(s) of an Item in a Checklist hierarchy
// Note: An item position can change when using Replace or Insert operations
public class PositionLocator : ChecklistVisitor {
    private readonly Item _item;
    private Position _position = new();
    private List<Position> _itemPositions = [];

    public List<Position> Results => _itemPositions;

    public PositionLocator(Checklist checklist, Item item) {
        _item = item;
        checklist.Accept(this);
    }

    public void Visit(BooleanItem item, string question, bool? value, Dictionary<Person, List<Operation>> operations) {
        if (item == _item) _itemPositions.Add(_position);
        _position.Increment();
    }

    public void Visit(MultipleChoiceItem item,
        string question,
        object? value,
        List<object> choices,
        Dictionary<Person, List<Operation>> operations) {
        if (item == _item) _itemPositions.Add(_position);
        _position.Increment();
    }

    public void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {
        if (item == _item) _itemPositions.Add(_position);
        _position.Deeper();
    }

    public void PostVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {
        _position.Truncate();
        _position.Increment();
    }

    public void PreVisit(NotItem item, Item negatedItem) {
        if (item == _item) _itemPositions.Add(_position);
        _position.Deeper();
    }

    public void PostVisit(NotItem item, Item negatedItem) {
        _position.Truncate();
        _position.Increment();
    }

    public void PreVisit(OrItem item, Item item1, Item item2) {
        if (item == _item) _itemPositions.Add(_position);
        _position.Deeper();
    }

    public void PostVisit(OrItem item, Item item1, Item item2) {
        _position.Truncate();
        _position.Increment();
    }

    public void PreVisit(GroupItem item, List<Item> childItems) {
        if (item == _item) _itemPositions.Add(_position);
        _position.Deeper();
    }

    public void PostVisit(GroupItem item, List<Item> childItems) {
        _position.Truncate();
        _position.Increment();
    }
}