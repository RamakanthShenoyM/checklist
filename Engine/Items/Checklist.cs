﻿using Engine.Persons;
using static Engine.Persons.Role;

namespace Engine.Items {
    public class Checklist {
        private Item _item;
        private readonly Person _creator;

        public Checklist(Person creator, Item firstItem, params Item[] items) {
            _item = (items.Length == 0) ? firstItem : new GroupItem(firstItem, items);
            _creator = creator;
            _item.AddPerson(_creator, Creator);
        }

        public void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _creator);
            _item.Accept(visitor);
            visitor.PostVisit(this, _creator);
        }

        public ChecklistStatus Status() {
            if (_item.Status() == ItemStatus.Succeeded)
                return ChecklistStatus.Succeeded;
            if (_item.Status() == ItemStatus.Failed)
                return ChecklistStatus.Failed;
            return ChecklistStatus.InProgress;
        }

        internal bool HasCreator(Person person) => person == _creator;

        public override string ToString() => ToString(true);

        public string ToString(bool showOperations) => new PrettyPrint(this, showOperations).Result();

        public void Replace(Item originalItem, Item newItem) {
            newItem.AddPerson(_creator, Creator);
            if (_item == originalItem) {
                _item = newItem;
                return;
            }

            if (!_item.Replace(originalItem, newItem))
                throw new InvalidOperationException("Item not found in checklist");
        }

        public void Simplify() {
            _item.Simplify();
        }

        internal void Remove(Item item) {
            if (item == _item) throw new InvalidOperationException("Cannot remove the only item in the checklist");
            if (!_item.Remove(item)) throw new InvalidOperationException("Item not found in checklist");
        }

        public Item I(int firstIndex, params int[] rest) {
            if (firstIndex != 0) throw new InvalidOperationException(
                "There is only one item at the root of the Checklist hierarchy, so use index 0.");
            var indexes = rest.ToList();
            indexes.Insert(0, firstIndex);
            return _item.I(indexes);
        }

        public string ToMemento() => new ChecklistSerializer(this).Result;

        public static Checklist FromMemento(string memento) => 
            new ChecklistDeserializer(memento).Result;
    }
}