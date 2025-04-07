using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class NullItem : Item
    {
        public static readonly NullItem Instance = new();
        internal override void Accept(ChecklistVisitor visitor) => visitor.Visit(this);

        internal override void Be(object value) { }

        internal override Item I(List<int> indexes) => this;

        internal override void Reset() { }

        internal override ItemStatus Status() => Succeeded;
        internal override void AddPerson(Person person, Role role, History history)
        {
            //Ignore this
        }
    }
}
