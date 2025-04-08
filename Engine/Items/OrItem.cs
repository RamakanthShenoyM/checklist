using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class OrItem : Item
    {
        private Item _item1;
        private Item _item2;

        // Use extension method to create an OrItem
        internal OrItem(Item item1, Item item2)
        {
            _item1 = item1;
            _item2 = item2;
        }
        
        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _item1, _item2, Operations);
            _item1.Accept(visitor);
            _item2.Accept(visitor);
            visitor.PostVisit(this, _item1, _item2, Operations);
        }

        internal override void Be(object value)
        {
            throw new InvalidOperationException("can't set the Or");
        }

        internal override void Reset()
        {
            throw new InvalidOperationException("can't reset the Or");
        }

        public override bool Equals(object? obj) => this == obj || obj is OrItem other && this.Equals(other);

        private bool Equals(OrItem other) =>
            this._item1.Equals(other._item1) && this._item2.Equals(other._item2) &&
            this.Operations.DeepEquals(other.Operations);

        public override int GetHashCode() => _item1.GetHashCode() + _item2.GetHashCode();

        internal override bool Replace(Item originalItem, Item newItem)
        {
            var result = false;
            if (_item1 == originalItem)
            {
                _item1 = newItem;
                result = true;
            }
            if (_item2 == originalItem)
            {
                _item2 = newItem;
                result = true;
            }
            result = _item1.Replace(originalItem, newItem) || result;
            return _item2.Replace(originalItem, newItem) || result;
        }

        internal override ItemStatus Status()
        {
            if (_item1.Status() == Failed || _item2.Status() == Failed) return Failed;
            if (_item1.Status() == Succeeded || _item2.Status() == Succeeded) return Succeeded;
            return Unknown;
        }

        internal override void AddPerson(Person person, Role role)
        {
            base.AddPerson(person, role);
            _item1.AddPerson(person, role);
            _item2.AddPerson(person, role);
        }

        internal override void History(History history)
        {
            base.History(history);
            _item1.History(history);
            _item2.History(history);
        }

        internal override bool Contains(Item desiredItem) =>
           _item1.Contains(desiredItem)
               || _item2.Contains(desiredItem);

        internal override void Simplify() {
            _item1.Simplify();
            _item2.Simplify();
        }

        internal override bool Remove(Item item)
        {
            if (_item1 == item || _item2 == item) throw new InvalidOperationException("Cannot remove items in OrItem");

            var result = _item1.Remove(item);
            return _item2.Remove(item) || result;
        }

        internal override Item I(List<int> indexes) {
            if (indexes.Count == 1) return this;
            if (indexes[1] == 0) return _item1.I(indexes.Skip(1).ToList());
            if (indexes[1] == 1) return _item2.I(indexes.Skip(1).ToList());
            throw new InvalidOperationException($"Invalid index of {indexes[1]} for an OrItem. Should be 0 or 1 only.");
        }

        internal override List<SimpleItem> ActiveItems() => [.._item1.ActiveItems().Concat(_item2.ActiveItems())];
    }
}
