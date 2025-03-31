using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items {
    public class ConditionalItem : Item {
        private Item _baseItem;
        private Item? _successItem;
        private Item? _failItem;

        // Use extension method to create a ConditionalItem
        internal ConditionalItem(Item condition, Item? onSuccess = null, Item? onFail = null) {
            _baseItem = condition;
            _successItem = onSuccess;
            _failItem = onFail;
        }

        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _baseItem, _successItem, _failItem);
            _baseItem.Accept(visitor);
            _successItem?.Accept(visitor);
            _failItem?.Accept(visitor);
            visitor.PostVisit(this, _baseItem, _successItem, _failItem);
        }

        internal override void Be(object value) =>
            throw new InvalidOperationException("can't set the Conditional Item");

        internal override void Reset() => throw new InvalidOperationException("can't set the Conditional Item");

        internal override bool Replace(Item originalItem, Item newItem) {
            var result = Replace(ref _baseItem, originalItem, newItem);
            result = Replace(ref _successItem, originalItem, newItem) || result;
            return Replace(ref _failItem, originalItem, newItem) || result;
        }

        private bool Replace(ref Item? currentItem, Item originalItem, Item newItem) {
            if (currentItem == null) return false;
            if (currentItem == originalItem) {
                currentItem = newItem;
                return true;
            }

            return currentItem.Replace(originalItem, newItem);
        }

        internal override ItemStatus Status() {
            if (_baseItem.Status() == Succeeded) return _successItem?.Status() ?? Succeeded;
            if (_baseItem.Status() == Failed) return _failItem?.Status() ?? Failed;
            return Unknown;
        }

        internal override void AddPerson(Person person, Role role) {
            base.AddPerson(person, role);
            _baseItem.AddPerson(person, role);
            _successItem?.AddPerson(person, role);
            _failItem?.AddPerson(person, role);
        }

        internal override bool Contains(Item desiredItem) =>
            _baseItem.Contains(desiredItem)
            || (_successItem?.Contains(desiredItem) ?? false)
            || (_failItem?.Contains(desiredItem) ?? false);

        internal override void Simplify() {
            _baseItem.Simplify();
            _successItem?.Simplify();
            _failItem?.Simplify();
        }

        internal override bool Remove(Item item) {
            var result = false;

            if (_baseItem == item) throw new InvalidOperationException("Cannot remove the base item");
            
            if (_successItem == item) {
                if (_failItem == null)
                    throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                _successItem = null;
                result = true;
            }

            if (_failItem == item) {
                if (_successItem == null)
                    throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                _failItem = null;
                result = true;
            }

            var baseResult = _baseItem.Remove(item);
            var successResult = _successItem?.Remove(item) ?? false;
            var failItemResult = _failItem?.Remove(item) ?? false;

            return result || baseResult || successResult || failItemResult;
        }

        internal override Item I(List<int> indexes) {
            if (indexes.Count == 1) return this;
            return indexes[1] switch {
                0 => _baseItem.I(indexes.Skip(1).ToList()),
                1 => _successItem?.I(indexes.Skip(1).ToList()) ?? Fail("success"),
                2 => _failItem?.I(indexes.Skip(1).ToList()) ?? Fail("failure"),
                _ => throw new InvalidOperationException("Invalid index for a Conditional Item")
            };
        }

        private Item Fail(string legName) {
            throw new InvalidOperationException($"No {legName} defined for this Conditional Item");
        }
    }
}