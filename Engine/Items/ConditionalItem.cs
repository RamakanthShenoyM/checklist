using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class ConditionalItem(Item baseItem, Item? successItem = null, Item? failItem = null) : Item
    {

        internal override void Accept(ChecklistVisitor visitor)
        {
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
            var result = Replace(ref baseItem, originalItem, newItem);
            result = Replace(ref successItem, originalItem, newItem) || result;
           return Replace(ref failItem, originalItem, newItem) || result;
        }
        private bool Replace(ref Item? currentItem, Item originalItem, Item newItem)
        {
            if (currentItem == null) return false;
            if (currentItem == originalItem)
            {
                currentItem = newItem;
                return true;
            }

            return currentItem.Replace(originalItem, newItem);

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

        internal override void Simplify()
        {
            baseItem.Simplify();
            successItem?.Simplify();
            failItem?.Simplify();
        }

        internal override bool Remove(Item item)
        {
            var result = false;

            if (baseItem == item) throw new InvalidOperationException("Cannot remove the base item");
            if (successItem == item)
            {
                if (failItem == null) throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                successItem = null;
                result = true;
            }

            if (failItem == item)
            {
                if (successItem == null) throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                failItem = null;
                result = true;
            }

            var baseResult = baseItem.Remove(item);
            var successResult = successItem?.Remove(item) ?? false;
            var failItemResult = failItem?.Remove(item) ?? false;

            return result || baseResult || successResult || failItemResult;
        }

        internal override Item I(List<int> indexes) {
            throw new NotImplementedException();
        }
    }
}