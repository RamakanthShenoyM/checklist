﻿using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items
{
    public class GroupItem : Item
    {
        private readonly List<Item> _childItems;

        // Use extension method to create a GroupItem
        internal GroupItem(Item firstItem, params Item[] items) // Trick to ensure one Item exists
        {
            _childItems = items.ToList();
            _childItems.Insert(0, firstItem);
        }

        internal GroupItem(List<Item> items) => _childItems = items;

        protected override List<Item> SubItems() => [.._childItems];

        internal override ItemStatus Status()
        {
            if (_childItems.Count == 0) return ItemStatus.Unknown;
            var statuses = _childItems.Select(item => item.Status()).ToList();
            if (statuses.All(status => status == ItemStatus.Succeeded))
                return ItemStatus.Succeeded;
            if (statuses.Any(status => status == ItemStatus.Failed))
                return ItemStatus.Failed;
            return ItemStatus.Unknown;
        }
        
        public override bool Equals(object? obj) => this == obj || obj is GroupItem other && this.Equals(other);

        private bool Equals(GroupItem other) =>
            this._childItems.SequenceEqual(other._childItems) && this.Operations.DeepEquals(other.Operations);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var item in _childItems) hash.Add(item);
            return hash.ToHashCode();
        }

        internal override bool Replace(Item originalItem, Item newItem)
        {
            var result = false;
            if (_childItems.Contains(originalItem))
            {
                var index = _childItems.IndexOf(originalItem);
                _childItems.RemoveAt(index);
                _childItems.Insert(index, newItem);
                result = true;
            }
            foreach (var item in _childItems) result = item.Replace(originalItem, newItem) || result;
            return result;
        }

        internal override void Accept(ChecklistVisitor visitor)
        {
            visitor.PreVisit(this, _position, _childItems, Operations);
            foreach (var item in _childItems) item.Accept(visitor);
            visitor.PostVisit(this, _position, _childItems, Operations);
        }

        internal override void AddPerson(Person person, Role role)
        {
            base.AddPerson(person, role);
            foreach (var item in _childItems) item.AddPerson(person, role);
        }

        internal override void History(History history)
        {
            base.History(history);
            foreach (var item in _childItems) item.History(history);
        }
        
        internal override void RemovePerson(Person person)
        {
            base.RemovePerson(person);
            foreach (var item in _childItems) item.RemovePerson(person);
        }

        internal override bool Contains(Item desiredItem) =>
            _childItems.Contains(desiredItem);

        internal override void Simplify()
        {
            var originalItems = new List<Item>(_childItems);
            foreach (var item in originalItems)
            {
                if (item is GroupItem)
                {
                    var index = _childItems.IndexOf(item);
                    _childItems.RemoveAt(index);
                    _childItems.InsertRange(index, ((GroupItem)item)._childItems);
                }
            }

            foreach (var item in _childItems) item.Simplify();
        }

        internal override bool Remove(Item item)
        {
            if (_childItems.Contains(item))
            {
                if (_childItems.Count == 1) throw new InvalidOperationException("Cannot remove the only item in the checklist");
                _childItems.Remove(item);
                foreach (var childItem in _childItems) childItem.Remove(item);
                return true;
            }

            var result = false;
            foreach (var childItem in _childItems) result = childItem.Remove(item) || result;

            return result;
        }

        internal override List<SimpleItem> ActiveItems() => 
            _childItems.SelectMany(item => item.ActiveItems()).ToList();

        internal override Item Clone()
        {
            var clonedItem = _childItems.Select(x => x.Clone()).ToList();
            return new GroupItem(clonedItem[0], clonedItem.Skip(1).ToArray());
        }
    }
}
