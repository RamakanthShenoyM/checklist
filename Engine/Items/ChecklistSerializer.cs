using Engine.Persons;
using System.Text.Json;

namespace Engine.Items
{
    internal class ChecklistSerializer: ChecklistVisitor
    {
        private readonly List<ItemDto> _dtos = [];
        private readonly Position _position = new();
        internal ChecklistSerializer(Checklist checklist)
        {
            checklist.Accept(this);
        }

        internal string Result
        {
            get
            {
                if (_dtos.Count == 0) throw new InvalidOperationException(
                    "The serializer has not examined the Checklist. Visitor issue possible?");
                return JsonSerializer.Serialize(_dtos);
            }
        }

        public void Visit(BooleanItem item,Guid id, string question, bool? value, Dictionary<Person, List<Operation>> operations)
        {
            _dtos.Add(new ItemDto(
                nameof(BooleanItem),
                _position.ToString(),
                id,
                question, 
                new ValueDto(typeof(Boolean).ToString(), value.ToString())));
            _position.Increment();
        }

        public void Visit(NullItem item)
        {
            _dtos.Add(new ItemDto(nameof(NullItem), _position.ToString()));
            _position.Increment();
        }

        public void Visit(MultipleChoiceItem item,Guid id, string question, object? value,List<object> choices, 
            Dictionary<Person, List<Operation>> operations)
        {
            _dtos.Add(new ItemDto(
                nameof(MultipleChoiceItem), 
                _position.ToString(), 
                id,
                question, 
                new ValueDto(value?.GetType().ToString(), value?.ToString()?? ""),
                choices.Select(c => new ValueDto(c.GetType().ToString(), c.ToString())).ToList()));
            _position.Increment();
        }

        public void PreVisit(GroupItem item, List<Item> childItems) => _position.Deeper();
        public void PostVisit(GroupItem item, List<Item> childItems) => CreateComposite(item);
        public void PreVisit(OrItem item, Item item1, Item item2) => _position.Deeper();
        public void PostVisit(OrItem item, Item item1, Item item2) => CreateComposite(item);
        public void PreVisit(NotItem item, Item negatedItem) => _position.Deeper();
        public void PostVisit(NotItem item, Item negatedItem) => CreateComposite(item);
        public void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) => _position.Deeper();
        public void PostVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) => 
            CreateComposite(item);
        private void CreateComposite(Item item)
        {
            _position.Truncate();
            _dtos.Add(new ItemDto(item.GetType().Name, _position.ToString()));
            _position.Increment();
        }
    }

    public record ItemDto(
        string ItemClassName,
        string Position, 
        Guid? Id = null,
        string? Question = null, 
        ValueDto? Value=null,
        List<ValueDto>? Choices = null);

    public record ValueDto(string? ValueClass, string? ValueValue);

}
