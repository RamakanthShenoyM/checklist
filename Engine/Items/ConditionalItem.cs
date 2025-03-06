using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class ConditionalItem(Item baseItem, Item? successItem = null, Item? failItem = null) : Item
    {

        internal override void Be(object value) => baseItem.Be(value);

        internal override void Reset()
        {
            baseItem.Reset();
        }

        internal override ItemStatus Status()
        {
            if (baseItem.Status() == Succeeded) return successItem?.Status() ?? Succeeded;
            if (baseItem.Status() == Failed) return failItem?.Status() ?? Failed;
            return Unknown;
        }

        internal override void AddPerson(Person person, Role role)
        {
            baseItem.AddPerson(person, role);
            successItem?.AddPerson(person, role);
            failItem?.AddPerson(person, role);
        }
        internal override bool Contains(Item desiredItem) =>
            baseItem.Contains(desiredItem)
                || (successItem?.Contains(desiredItem) ?? false)
                || (failItem?.Contains(desiredItem) ?? false);
    }
}