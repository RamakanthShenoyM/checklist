using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class ConditionalItem(Item baseItem, Item? successItem = null, Item? failItem = null) : Item
    {

        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, baseItem, successItem, failItem);
            baseItem.Accept(visitor);
            successItem?.Accept(visitor);
            failItem?.Accept(visitor);
            visitor.PostVisit(this, baseItem, successItem, failItem);
        }

        internal override void Be(object value) => throw new InvalidOperationException("can't set the Conditional Item");

        internal override void Reset() => throw new InvalidOperationException("can't set the Conditional Item");
        internal override bool Replace(Item originalItem, Item newItem)
        {
            if (baseItem == originalItem)
            {
                baseItem = newItem;
                return true;
            }
            if (successItem == originalItem)
            {
                successItem = newItem;
                return true;
            }
            if (failItem == originalItem)
            {
                failItem = newItem;
                return true;
            }
            return new List<Item>{baseItem,successItem,failItem}.Any(item => item.Replace(originalItem, newItem));
        }
        internal override ItemStatus Status()
        {
            if (baseItem.Status() == Succeeded) return successItem?.Status() ?? Succeeded;
            if (baseItem.Status() == Failed) return failItem?.Status() ?? Failed;
            return Unknown;
        }

        internal override void AddPerson(Person person, Role role)
        {
            base.AddPerson(person, role);
            baseItem.AddPerson(person, role);
            successItem?.AddPerson(person, role);
            failItem?.AddPerson(person, role);
        }
        
        internal override bool Contains(Item desiredItem) =>
            baseItem.Contains(desiredItem)
                || (successItem?.Contains(desiredItem) ?? false)
                || (failItem?.Contains(desiredItem) ?? false);

        internal override void Simplify() {
            baseItem.Simplify();
            successItem?.Simplify();
            failItem?.Simplify();
        }

        internal override bool Remove(Item item)
        {
            if (baseItem == item) throw new InvalidOperationException("Cannot remove the base item");
            
            var baseResult = baseItem.Remove(item);
            var successResult = successItem?.Remove(item) ?? false;
            var failItemResult = failItem?.Remove(item) ?? false;

            return baseResult || successResult || failItemResult;
        }
    }
}