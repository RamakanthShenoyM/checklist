namespace Engine.Items
{
    public class MultipleChoiceItem: Item
    {
        private readonly List<object> _choices;
        private object? _value;
        
        public MultipleChoiceItem(object firstChoice, params object[] choices)
        {
            _choices = choices.ToList();
            _choices.Insert(0, firstChoice);
        }
        
        internal override void Accept(ChecklistVisitor visitor) {
            visitor.Visit(this, _value, Operations);
        }

        internal override void Be(object value) => _value = value;

        internal override void Reset() => _value = null;

        internal override ItemStatus Status()
        {
            if (_value == null) return ItemStatus.Unknown;
            return _choices.Contains(_value)? ItemStatus.Succeeded: ItemStatus.Failed;
        }
    }
}
