using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class OrItem : Item
    {
        private readonly Item _item1;
        private readonly Item _item2;

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

        internal override ItemStatus Status()
        {
            if (_item1.Status() == Failed || _item2.Status() == Failed) return Failed;
            if (_item1.Status() == Succeeded || _item2.Status() == Succeeded) return Succeeded;
            return Unknown;
        }

        internal override void AddPerson(Person person, Role role)
        {
            _item1.AddPerson(person, role);
            _item2.AddPerson(person, role);
        }
        
        internal override bool Contains(Item desiredItem) =>
           _item1.Contains(desiredItem)
               || _item2.Contains(desiredItem);

    }
}
