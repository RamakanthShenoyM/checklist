using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class NotItem : Item
    {
        private Item _item;
        
        // Use extension method to create a new NotItem
        internal NotItem(Item item)
        {
            _item = item;
        }
        
        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _item, Operations);
            _item.Accept(visitor);
            visitor.PostVisit(this, _item, Operations);
        }

        internal override void Be(object value) => throw new InvalidOperationException("can't set the Not");

        internal override void Reset() => throw new InvalidOperationException("can't Reset the Not");

        public override bool Equals(object? obj) => this == obj || obj is NotItem other && this.Equals(other);

        private bool Equals(NotItem other) =>
            this._item.Equals(other._item) && this.Operations.DeepEquals(other.Operations);

        public override int GetHashCode() => _item.GetHashCode();

        internal override bool Replace(Item originalItem, Item newItem)
        {
            if (_item == originalItem)
            {
                _item = newItem;
                return true;
            }
            return _item.Replace(originalItem, newItem);
        }

        internal override ItemStatus Status()
        {
            if (_item.Status() == Succeeded) return Failed;
            if (_item.Status() == Failed) return Succeeded;
            return Unknown;
        }
        
        internal override void AddPerson(Person person, Role role)
        {
            base.AddPerson(person, role);
            _item.AddPerson(person, role);
        }

        internal override void History(History history)
        {
            base.History(history);  
            _item.History(history);
        }

        internal override bool Contains(Item desiredItem) =>
           _item.Contains(desiredItem);

        internal override void Simplify() => _item.Simplify();

        internal override bool Remove(Item item)
        {
            if (_item == item) throw new InvalidOperationException("Cannot remove items in OrItem");

            return _item.Remove(item);
        }

        internal override Item I(List<int> indexes) {
            if (indexes.Count == 1) return this;
            if (indexes[1] == 0) return _item.I(indexes.Skip(1).ToList());
            throw new InvalidOperationException($"Invalid index of {indexes[1]} for a NotItem. Should be 0.");
        }

        internal override List<SimpleItem> ActiveItems() => _item.ActiveItems();
    }
}
