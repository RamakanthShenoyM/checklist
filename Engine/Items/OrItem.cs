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
            visitor.PreVisit(this, _position, _item1, _item2, Operations);
            _item1.Accept(visitor);
            _item2.Accept(visitor);
            visitor.PostVisit(this, _position, _item1, _item2, Operations);
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

        protected override List<Item> SubItems() => [_item1, _item2];

        internal override ItemStatus Status()
        {
            if (_item1.Status() == Failed || _item2.Status() == Failed) return Failed;
            if (_item1.Status() == Succeeded || _item2.Status() == Succeeded) return Succeeded;
            return Unknown;
        }

        internal override void AddPerson(Person person, Role role)
        {
            base.AddPerson(person, role);
            Apply(item => item.AddPerson(person, role));
        }

        internal override void History(History history)
        {
            base.History(history);
            Apply(item => item.History(history));
        }

        internal override void RemovePerson(Person person)
        {
             base.RemovePerson(person);
             Apply(item => item.RemovePerson(person));
        }

        private void Apply(Action<Item> action ) {
            action(_item1);
            action(_item2);
        }

        internal override bool Contains(Item desiredItem) =>
           _item1.Contains(desiredItem)
               || _item2.Contains(desiredItem);

        internal override void Simplify()
        {
            _item1.Simplify();
            _item2.Simplify();
        }

        internal override bool Remove(Item item)
        {
            if (_item1 == item || _item2 == item) throw new InvalidOperationException("Cannot remove items in OrItem");

            var result = _item1.Remove(item);
            return _item2.Remove(item) || result;
        }

        internal override List<SimpleItem> ActiveItems() => [.. _item1.ActiveItems().Concat(_item2.ActiveItems())];
        internal override Item Clone() => new OrItem(_item1.Clone(), _item2.Clone());
    }
}
