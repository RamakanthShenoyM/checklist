using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class NotItem : Item
    {
        private Item _item;
        
        internal NotItem(Item item)
        {
            _item = item;
        }
        
        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _item);
            _item.Accept(visitor);
            visitor.PostVisit(this, _item);
        }

        internal override void Be(object value) => throw new InvalidOperationException("can't set the Not");

        internal override void Reset() => throw new InvalidOperationException("can't Reset the Not");

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

        internal override bool Contains(Item desiredItem) =>
           _item.Contains(desiredItem);
    }
}
