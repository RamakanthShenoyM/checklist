using static System.String;

namespace Engine.Items
{
    public class MultipleChoiceItem : Item
    {
        private readonly Guid _id;
        private readonly List<object> _choices;
        private readonly string _question;
        private object? _value;

        public MultipleChoiceItem(string question, object firstChoice,Guid? id=null, params object[] choices)
        {
            _choices = choices.ToList();
            _choices.Insert(0, firstChoice);
            _question = question;
            _id = id ?? Guid.NewGuid();
        }

        internal override void Accept(ChecklistVisitor visitor)
        {
            visitor.Visit(this,_id, _question, _value, _choices, Operations);
        }

        internal override void Be(object value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.GetType() != _choices[0].GetType()) throw new InvalidOperationException(
                $"Unexpected value type of <{value.GetType()}>; expected type <{_choices[0].GetType()}>");
            _value = value;
        }

        internal override void Reset() => _value = null;

        public override bool Equals(object? obj) => this == obj || obj is MultipleChoiceItem other && this.Equals(other);

        private bool Equals(MultipleChoiceItem other) =>
            Equals(this._value,other._value)&& this._question == other._question && this._choices.SequenceEqual(other._choices) && this._id == other._id;

        public override int GetHashCode() => HashCode.Combine(_question, _id, Join(",", _choices));

        internal override ItemStatus Status()
        {
            if (_value == null) return ItemStatus.Unknown;
            return _choices.Contains(_value) ? ItemStatus.Succeeded : ItemStatus.Failed;
        }

        internal override Item I(List<int> indexes)
        {
            if (indexes.Count == 1) return this;
            throw new InvalidOperationException($"No more items exist to reach with indexes {indexes}");
        }
    }
}
