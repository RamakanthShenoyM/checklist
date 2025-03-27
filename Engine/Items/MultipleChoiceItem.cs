namespace Engine.Items
{
    public class MultipleChoiceItem: Item
    {
        private readonly List<object> _choices;
		    private readonly string _question;
		    private object? _value;
        
        public MultipleChoiceItem(string question,object firstChoice, params object[] choices)
        {
            _choices = choices.ToList();
            _choices.Insert(0, firstChoice);
			  _question = question;
		}
        
        internal override void Accept(ChecklistVisitor visitor) {
            visitor.Visit(this, _question, _value, _choices, Operations);
        }

        internal override void Be(object value) => _value = value;

        internal override void Reset() => _value = null;

        internal override ItemStatus Status()
        {
            if (_value == null) return ItemStatus.Unknown;
            return _choices.Contains(_value)? ItemStatus.Succeeded: ItemStatus.Failed;
        }

        internal override Item I(List<int> indexes) {
            if (indexes.Count == 1) return this;
            throw new InvalidOperationException($"No more items exist to reach with indexes {indexes}");
        }
    }
}
