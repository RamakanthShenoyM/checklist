using CommonUtilities.Util;
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

    public void Visit(BooleanItem item,
        Guid id,
        Position position,
        string question,
        bool? value,
        Dictionary<Person, List<Operation>> operations,
        History history) {
        if (item == _item) _itemPositions.Add(_position.Clone());
        _position.Increment();
    }

    public void Visit(MultipleChoiceItem item,
        Guid id,
        Position position,
        string question,
        object? value,
        List<object> choices,
        Dictionary<Person, List<Operation>> operations,
        History history) {
        if (item == _item) _itemPositions.Add(_position.Clone());
        _position.Increment();
    }

    public void PreVisit(ConditionalItem item,
        Position position,
        Item baseItem,
        Item? successItem,
        Item? failureItem,
        Dictionary<Person, List<Operation>> operations) {
        if (item == _item) _itemPositions.Add(_position.Clone());
        _position.Deeper();
    }

    public void PostVisit(ConditionalItem item,
        Position position,
        Item baseItem,
        Item? successItem,
        Item? failureItem,
        Dictionary<Person, List<Operation>> operations) {
        _position.Truncate();
        _position.Increment();
    }

    public void PreVisit(NotItem item,
        Position position,
        Item negatedItem,
        Dictionary<Person, List<Operation>> operations) {
        if (item == _item) _itemPositions.Add(_position.Clone());
        _position.Deeper();
    }

    public void PostVisit(NotItem item,
        Position position,
        Item negatedItem,
        Dictionary<Person, List<Operation>> operations) {
        _position.Truncate();
        _position.Increment();
    }

    public void PreVisit(OrItem item,
        Position position,
        Item item1,
        Item item2,
        Dictionary<Person, List<Operation>> operations) {
        if (item == _item) _itemPositions.Add(_position.Clone());
        _position.Deeper();
    }

    public void PostVisit(OrItem item,
        Position position,
        Item item1,
        Item item2,
        Dictionary<Person, List<Operation>> operations) {
        _position.Truncate();
        _position.Increment();
    }

    public void PreVisit(GroupItem item,
        Position position,
        List<Item> childItems,
        Dictionary<Person, List<Operation>> operations) {
        if (item == _item) _itemPositions.Add(_position.Clone());
        _position.Deeper();
    }

    public void PostVisit(GroupItem item,
        Position position,
        List<Item> childItems,
        Dictionary<Person, List<Operation>> operations) {
        _position.Truncate();
        _position.Increment();
    }
}