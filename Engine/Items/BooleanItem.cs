using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items {
    public class BooleanItem : SimpleItem {
        private readonly Guid _id;
        private bool? _hasSucceeded;
        private readonly string _question;

        public BooleanItem(string question, Guid? id = null)
		{
			_question = question;
            _id = id??Guid.NewGuid();
        }


        internal override void Be(object value) {
            ArgumentNullException.ThrowIfNull(value);
            _hasSucceeded = (bool)value;
        }

        public override bool Equals(object? obj) => this == obj || obj is BooleanItem other && this.Equals(other);

        private bool Equals(BooleanItem other) =>
            this._hasSucceeded == other._hasSucceeded
            && this._question == other._question
            && this._id == other._id
            && this.Operations.DeepEquals(other.Operations)
            && this.History().Equals(other.History());

        public override int GetHashCode() => _question.GetHashCode() * 37 + _id.GetHashCode();

        internal override void Reset() => _hasSucceeded = null;

        internal override void Accept(ChecklistVisitor visitor) {
	        visitor.Visit(this,_id, _position, _question, _hasSucceeded, Operations,History());
        }

        internal override ItemStatus Status() => _hasSucceeded switch {
            true => ItemStatus.Succeeded,
            false => ItemStatus.Failed,
            _ => ItemStatus.Unknown,
        };

        internal override List<SimpleItem> ActiveItems() => [this];
        internal override Item Clone()
        {
            var result = new BooleanItem(_question, _id);
            result._hasSucceeded = _hasSucceeded;
            return result;
        }

        public override string ToString() => _question;
    }
}