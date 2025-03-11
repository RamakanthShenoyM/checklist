using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Persons;

namespace Engine.Items
{
    public class GroupItem:Item
    {
        private readonly List<Item> _childItems = [];

        public GroupItem(Item firstItem, params Item[] items) 
        {
            _childItems= items.ToList();
            _childItems.Insert(0, firstItem);
        }
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

        internal override void Be(object value) => throw new InvalidOperationException("can't set the Group Item");

        internal override void Reset() => throw new InvalidOperationException("can't reset the Group Item");

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
            visitor.PreVisit(this, _childItems);
            foreach (var item in _childItems) item.Accept(visitor) ;
            visitor.PostVisit(this,_childItems);
        }

        internal override void AddPerson(Person person, Role role)
        {
            base.AddPerson(person, role);
            foreach (var item in _childItems) item.AddPerson(person, role);
        }

        internal override bool Contains(Item desiredItem) =>
            _childItems.Contains(desiredItem);

        internal override void Simplify() {
            var originalItems = new List<Item>(_childItems);
            foreach (var item in originalItems) {
                if (item is GroupItem) {
                    var index = _childItems.IndexOf(item);
                    _childItems.RemoveAt(index);
                    _childItems.InsertRange(index, ((GroupItem)item)._childItems);
                }
            }

            foreach (var item in _childItems) item.Simplify();
        }

        internal override bool Remove(Item item)
        {
            if (_childItems.Contains(item)) {
                if (_childItems.Count == 1) throw new InvalidOperationException("Cannot remove the only item in the checklist");
                _childItems.Remove(item);
                foreach(var childItem in _childItems) childItem.Remove(item);
                return true;
            }
            
            var result = false;
            foreach (var childItem in _childItems) result = childItem.Remove(item) || result;

            return result;
        }
    }
}
