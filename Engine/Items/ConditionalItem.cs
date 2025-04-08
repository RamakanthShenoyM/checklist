using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.ItemExtensions;
using static Engine.Items.ItemStatus;

namespace Engine.Items {
    public class ConditionalItem : Item {
        private Item _conditionItem;
        private Item _onSuccessItem;
        private Item _onFailItem;

        // Use extension method to create a ConditionalItem
        internal ConditionalItem(Item condition, Item? onSuccess = null, Item? onFail = null) {
            _conditionItem = condition;
            _onSuccessItem = onSuccess ?? NullItem.Instance;
            _onFailItem = onFail ?? NullItem.Instance;
        }

        internal override void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _conditionItem, _onSuccessItem, _onFailItem, Operations);
            _conditionItem.Accept(visitor);
            _onSuccessItem.Accept(visitor);
            _onFailItem.Accept(visitor);
            visitor.PostVisit(this, _conditionItem, _onSuccessItem, _onFailItem, Operations);
        }

        internal override void Be(object value) =>
            throw new InvalidOperationException("can't set the Conditional Item");

        internal override void Reset() => throw new InvalidOperationException("can't set the Conditional Item");

        public override bool Equals(object? obj) => this == obj || obj is ConditionalItem other && this.Equals(other);

        private bool Equals(ConditionalItem other) =>
            this._conditionItem.Equals(other._conditionItem)
            && (this._onSuccessItem?.Equals(other._onSuccessItem) ?? other._onSuccessItem == null)
            && (this._onFailItem?.Equals(other._onFailItem) ?? other._onFailItem == null)
            && this.Operations.DeepEquals(other.Operations);

        public override int GetHashCode() => _conditionItem.GetHashCode() + (_onSuccessItem?.GetHashCode() ?? 0) + (_onFailItem?.GetHashCode() ?? 0);

        internal override bool Replace(Item originalItem, Item newItem) {
                #pragma warning disable CS8601 // Possible null reference assignment.
            var result = Replace(ref _conditionItem, originalItem, newItem);
                #pragma warning restore CS8601 // Possible null reference assignment.
            result = Replace(ref _onSuccessItem, originalItem, newItem) || result;
            return Replace(ref _onFailItem, originalItem, newItem) || result;
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
            if (_conditionItem.Status() == Succeeded) return (_onSuccessItem is NullItem)? Succeeded 
                    : _onSuccessItem.Status();
            if (_conditionItem.Status() == Failed) return (_onFailItem is NullItem) ? Failed : _onFailItem.Status();
            return Unknown;
        }

        internal override List<SimpleItem> ActiveItems() {
            var result = _conditionItem.ActiveItems();
            return _conditionItem.Status() switch {
                Unknown => result,
                Succeeded => [..result.Concat(_onSuccessItem.ActiveItems())],
                Failed => [..result.Concat(_onFailItem.ActiveItems())]
            };
        }

        internal override void AddPerson(Person person, Role role, History history) {
            base.AddPerson(person, role, history);
            _conditionItem.AddPerson(person, role, history);
            _onSuccessItem?.AddPerson(person, role, history);
            _onFailItem?.AddPerson(person, role, history);
        }

        internal override bool Contains(Item desiredItem) =>
            _conditionItem.Contains(desiredItem)
            || (_onSuccessItem?.Contains(desiredItem) ?? false)
            || (_onFailItem?.Contains(desiredItem) ?? false);

        internal override void Simplify() {
            _conditionItem.Simplify();
            _onSuccessItem?.Simplify();
            _onFailItem?.Simplify();
        }

        internal override bool Remove(Item item) {
            var result = false;

            if (_conditionItem == item) throw new InvalidOperationException("Cannot remove the base item");
            
            if (_onSuccessItem == item) {
                if (_onFailItem is NullItem)
                    throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                _onSuccessItem = NullItem.Instance;
                result = true;
            }

            if (_onFailItem == item) {
                if (_onSuccessItem is NullItem)
                    throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                _onFailItem = NullItem.Instance;
                result = true;
            }

            var baseResult = _conditionItem.Remove(item);
            var successResult = _onSuccessItem?.Remove(item) ?? false;
            var failItemResult = _onFailItem?.Remove(item) ?? false;

            return result || baseResult || successResult || failItemResult;
        }

        internal override Item I(List<int> indexes) {
            if (indexes.Count == 1) return this;
            return indexes[1] switch {
                0 => _conditionItem.I(indexes.Skip(1).ToList()),
                1 => _onSuccessItem?.I(indexes.Skip(1).ToList()) ?? Fail("success"),
                2 => _onFailItem?.I(indexes.Skip(1).ToList()) ?? Fail("failure"),
                _ => throw new InvalidOperationException("Invalid index for a Conditional Item")
            };
        }

        private Item Fail(string legName) {
            throw new InvalidOperationException($"No {legName} defined for this Conditional Item");
        }
    }
}