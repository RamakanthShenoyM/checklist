using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items {
    public class BooleanItem : SimpleItem {
        private readonly Guid _id;
        private bool? _hasSucceeded;
        private readonly string _question;
        private History? _history;

        public BooleanItem(string question, Guid? id = null) {
            _question = question;
            _id = id ?? Guid.NewGuid();
        }

        internal override History History() => 
            _history ?? throw new InvalidOperationException("History has not been initialized. Is it part of a Checklist?");

        internal override void Be(object value) {
            ArgumentNullException.ThrowIfNull(value);
            _hasSucceeded = (bool)value;
        }

        public override bool Equals(object? obj) => this == obj || obj is BooleanItem other && this.Equals(other);

        private bool Equals(BooleanItem other) =>
            this._hasSucceeded == other._hasSucceeded
            && this._question == other._question
            && this._id == other._id
            && this.Operations.DeepEquals(other.Operations);


        public override int GetHashCode() => _question.GetHashCode() * 37 + _id.GetHashCode();

        internal override void Reset() => _hasSucceeded = null;

        internal override void Accept(ChecklistVisitor visitor) {
            visitor.Visit(this, _id, _question, _hasSucceeded, Operations);
        }

        internal override Item I(List<int> indexes) {
            if (indexes.Count == 1) return this;
            throw new InvalidOperationException($"No more items exist to reach with indexes {indexes}");
        }

        internal override ItemStatus Status() => _hasSucceeded switch {
            true => ItemStatus.Succeeded,
            false => ItemStatus.Failed,
            _ => ItemStatus.Unknown,
        };

        internal override void AddPerson(Person person, Role role, History history) {
            _history = history;
            base.AddPerson(person, role, history);
        }

        public override string ToString() => _question;
    }
}