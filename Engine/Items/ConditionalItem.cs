using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class ConditionalItem : Item
    {
        private Item _conditionItem;
        private Item _onSuccessItem;
        private Item _onFailItem;

        // Use extension method to create a ConditionalItem
        internal ConditionalItem(Item condition, Item? onSuccess = null, Item? onFail = null)
        {
            _conditionItem = condition;
            _onSuccessItem = onSuccess ?? new NullItem();
            _onFailItem = onFail ?? new NullItem();
        }

        internal override void Accept(ChecklistVisitor visitor)
        {
            visitor.PreVisit(this, _position, _conditionItem, _onSuccessItem, _onFailItem, Operations);
            _conditionItem.Accept(visitor);
            _onSuccessItem.Accept(visitor);
            _onFailItem.Accept(visitor);
            visitor.PostVisit(this, _position, _conditionItem, _onSuccessItem, _onFailItem, Operations);
        }
        
        public override bool Equals(object? obj) => this == obj || obj is ConditionalItem other && this.Equals(other);

        private bool Equals(ConditionalItem other) =>
            this._conditionItem.Equals(other._conditionItem)
            && (this._onSuccessItem?.Equals(other._onSuccessItem) ?? other._onSuccessItem == null)
            && (this._onFailItem?.Equals(other._onFailItem) ?? other._onFailItem == null)
            && this.Operations.DeepEquals(other.Operations);

        public override int GetHashCode() => _conditionItem.GetHashCode() + (_onSuccessItem?.GetHashCode() ?? 0) + (_onFailItem?.GetHashCode() ?? 0);

        internal override bool Replace(Item originalItem, Item newItem)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            var result = Replace(ref _conditionItem, originalItem, newItem);
#pragma warning restore CS8601 // Possible null reference assignment.
            result = Replace(ref _onSuccessItem, originalItem, newItem) || result;
            return Replace(ref _onFailItem, originalItem, newItem) || result;
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

        protected override List<Item> SubItems() => [_conditionItem,  _onSuccessItem, _onFailItem];

        internal override ItemStatus Status()
        {
            if (_conditionItem.Status() == Succeeded) return (_onSuccessItem is NullItem) ? Succeeded
                    : _onSuccessItem.Status();
            if (_conditionItem.Status() == Failed) return (_onFailItem is NullItem) ? Failed : _onFailItem.Status();
            return Unknown;
        }

        internal override List<SimpleItem> ActiveItems()
        {
            var result = _conditionItem.ActiveItems();
            return _conditionItem.Status() switch
            {
                Unknown => result,
                Succeeded => [.. result.Concat(_onSuccessItem.ActiveItems())],
                Failed => [.. result.Concat(_onFailItem.ActiveItems())]
            };
        }

        internal override Item Clone() => 
            new ConditionalItem(_conditionItem.Clone(), _onSuccessItem.Clone(), _onFailItem.Clone());

        internal override void AddPerson(Person person, Role role)
        {
            base.AddPerson(person, role);
            Apply(item => item.AddPerson(person, role));
        }

        internal override void History(History history)
        {
            base.History(history);
            Apply(item => item.History(history));
        }

        internal override void RemovePerson(Person person)
        {
            base.RemovePerson(person);
            Apply(item => item.RemovePerson(person));
        }

        private void Apply(Action<Item> action ) {
            action(_conditionItem);
            action(_onSuccessItem);
            action(_onFailItem);
        }

        internal override bool Contains(Item desiredItem) =>
            _conditionItem.Contains(desiredItem)
            || (_onSuccessItem?.Contains(desiredItem) ?? false)
            || (_onFailItem?.Contains(desiredItem) ?? false);

        internal override void Simplify()
        {
            _conditionItem.Simplify();
            _onSuccessItem?.Simplify();
            _onFailItem?.Simplify();
        }

        internal override bool Remove(Item item)
        {
            var result = false;

            if (_conditionItem == item) throw new InvalidOperationException("Cannot remove the base item");

            if (_onSuccessItem == item)
            {
                if (_onFailItem is NullItem)
                    throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                _onSuccessItem = new NullItem();
                result = true;
            }

            if (_onFailItem == item)
            {
                if ( _onSuccessItem is NullItem)
                    throw new InvalidOperationException("Cannot remove the last leg in a conditional");
                _onFailItem = new NullItem();
                result = true;
            }

            var baseResult = _conditionItem.Remove(item);
            var successResult = _onSuccessItem?.Remove(item) ?? false;
            var failItemResult = _onFailItem?.Remove(item) ?? false;

            return result || baseResult || successResult || failItemResult;
        }

        private Item Fail(string legName)
        {
            throw new InvalidOperationException($"No {legName} defined for this Conditional Item");
        }
    }
}