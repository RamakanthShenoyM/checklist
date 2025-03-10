using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class OrItem : Item
    {
        private Item _item1;
        private Item _item2;

        internal OrItem(Item item1, Item item2)
        {
            _item1 = item1;
            _item2 = item2;
        }
        
        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _item1, _item2);
            _item1.Accept(visitor);
            _item2.Accept(visitor);
            visitor.PostVisit(this, _item1, _item2);
        }

        internal override void Be(object value)
        {
            throw new InvalidOperationException("can't set the Or");
        }

        internal override void Reset()
        {
            throw new InvalidOperationException("can't reset the Or");
        }

        internal override bool Replace(Item originalItem, Item newItem)
        {
            if (_item1 == originalItem)
            {
                _item1 = newItem;
                return true;
            }
            if (_item2 == originalItem)
            {
                _item2 = newItem;
                return true;
            }
            return new List<Item> { _item1, _item2 }.Any(item => item.Replace(originalItem, newItem));
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
        
        internal override bool Contains(Item desiredItem) =>
           _item1.Contains(desiredItem)
               || _item2.Contains(desiredItem);

    }
}
