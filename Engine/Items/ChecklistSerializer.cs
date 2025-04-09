using CommonUtilities.Util;
using Engine.Persons;
using System.Text.Json;

namespace Engine.Items
{
    internal class ChecklistSerializer: ChecklistVisitor
    {
        private readonly List<ItemDto> _dtos = [];
        private PersonDto _person;
        private readonly Position _position = new();
        private List<string> _events;
        internal ChecklistSerializer(Checklist checklist)
        {
            checklist.Accept(this);
        }

        internal string Result
        {
            get
            {
                if (_dtos.Count == 0 || _person is null) throw new InvalidOperationException(
                    "The serializer has not examined the Checklist. Visitor issue possible?");
                return JsonSerializer.Serialize(new CheckListDto(_person,_dtos,_events));
            }
        }

        public void Visit(History history, List<string> events) => _events = events;

        public void Visit(BooleanItem item,
            Guid id,
            Position position,
            string question,
            bool? value,
            Dictionary<Person, List<Operation>> operations,
            History history)
        {
            _dtos.Add(new ItemDto(
                nameof(BooleanItem),
                _position.ToString(),
                id,
                question, 
                new ValueDto(typeof(Boolean).ToString(), value.ToString()),
                Operations:OperationDtos(operations)));
            _position.Increment();
        }

        public void Visit(NullItem item)
        {
            _dtos.Add(new ItemDto(nameof(NullItem), _position.ToString()));
            _position.Increment();
        }

        public void Visit(MultipleChoiceItem item,
            Guid id,
            Position position,
            string question,
            object? value,
            List<object> choices,
            Dictionary<Person, List<Operation>> operations,
            History history)
        {
            _dtos.Add(new ItemDto(
                nameof(MultipleChoiceItem), 
                _position.ToString(), 
                id,
                question, 
                new ValueDto(value?.GetType().ToString(), value?.ToString()?? ""),
                choices.Select(c => new ValueDto(c.GetType().ToString(), c.ToString())).ToList(),
                Operations:OperationDtos(operations)));
            _position.Increment();
        }

        public void PreVisit(Checklist checklist, Person creator,History history) => 
            _person = new PersonDto(creator._organizationId, creator._personId);
        public void PreVisit(GroupItem item,
            Position position,
            List<Item> childItems,
            Dictionary<Person, List<Operation>> operations) => _position.Deeper();
        public void PostVisit(GroupItem item,
            Position position,
            List<Item> childItems,
            Dictionary<Person, List<Operation>> operations) => CreateComposite(item, operations);
        public void PreVisit(OrItem item,
            Position position,
            Item item1,
            Item item2,
            Dictionary<Person, List<Operation>> operations) => _position.Deeper();
        public void PostVisit(OrItem item,
            Position position,
            Item item1,
            Item item2,
            Dictionary<Person, List<Operation>> operations) => CreateComposite(item, operations);
        public void PreVisit(NotItem item,
            Position position,
            Item negatedItem,
            Dictionary<Person, List<Operation>> operations) => _position.Deeper();
        public void PostVisit(NotItem item,
            Position position,
            Item negatedItem,
            Dictionary<Person, List<Operation>> operations) => CreateComposite(item, operations);
        public void PreVisit(ConditionalItem item,
            Position position,
            Item baseItem,
            Item? successItem,
            Item? failureItem,
            Dictionary<Person, List<Operation>> operations) => _position.Deeper();
        public void PostVisit(ConditionalItem item,
            Position position,
            Item baseItem,
            Item? successItem,
            Item? failureItem,
            Dictionary<Person, List<Operation>> operations) => 
            CreateComposite(item,operations);
        private void CreateComposite(Item item, Dictionary<Person, List<Operation>> operations)
        {
            _position.Truncate();
            _dtos.Add(new ItemDto(item.GetType().Name, _position.ToString(),Operations: OperationDtos(operations)));
            _position.Increment();
        }
        private static List<OperationDto> OperationDtos(Dictionary<Person, List<Operation>> operations) =>
            operations.Select(o => new OperationDto(new PersonDto(o.Key._organizationId, o.Key._personId), o.Value)).ToList();
    }

    public record CheckListDto(PersonDto Person, List<ItemDto> Items,List<string> Events);

    public record ItemDto(
        string ItemClassName,
        string Position, 
        Guid? Id = null,
        string? Question = null, 
        ValueDto? Value=null,
        List<ValueDto>? Choices = null,
        List<OperationDto>? Operations=null);

    public record ValueDto(string? ValueClass, string? ValueValue);

    public record PersonDto(int OrganizationId, int PersonId);

    public record OperationDto(PersonDto Person, List<Operation> Operations);

}
