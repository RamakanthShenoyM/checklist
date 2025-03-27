using Engine.Persons;
using System.Text.Json;

namespace Engine.Items
{
    internal class ChecklistSerializer: ChecklistVisitor
    {
        private List<ItemDto> _dtos = [];

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

        public void Visit(BooleanItem item, string question, bool? value, Dictionary<Person, List<Operation>> operations)
        {
            _dtos.Add(new ItemDto(typeof(BooleanItem).ToString(), "0"));
        }
    }
    public class ItemDto(string itemClassName, string position)
    {
        public string ItemClassName => itemClassName;
        public string Position => position;
    }
}
