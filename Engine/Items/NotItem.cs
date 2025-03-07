using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class NotItem : Item
    {
        private readonly Item _item;
        
        internal NotItem(Item item)
        {
            _item = item;
        }
        
        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _item);
            _item.Accept(visitor);
            visitor.PostVisit(this, _item);
        }

        internal override void Be(object value) => _item.Be(value);

        internal override void Reset() => _item.Reset();

        internal override ItemStatus Status()
        {
            if (_item.Status() == Succeeded) return Failed;
            if (_item.Status() == Failed) return Succeeded;
            return Unknown;
        }
        
        internal override void AddPerson(Person person, Role role) => _item.AddPerson(person, role);
        
        internal override bool Contains(Item desiredItem) =>
           _item.Contains(desiredItem);
    }
}
