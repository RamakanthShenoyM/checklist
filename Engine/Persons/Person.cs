﻿using Engine.Items;
using static Engine.Persons.Operation;

namespace Engine.Persons
{
    public class Person
    {
        public Person(int organizationId, int personId)
        {

        }
        public AddingEngine Add(Person owner) => new(this, owner);

        public ActionEngine Can(Operation view) => new(this, view);

        public AssignEngine Sets(Item item) => new(this, item);

        public InsertEngine Insert(Item firstItem, params Item[] items) => 
            new(this, firstItem,items);

        public RemoveEngine Remove(Item item)
        {
            if (!Can(ModifyChecklist).On(item))
                throw new InvalidOperationException("Does not have permission to modify checklist");
            return new(item);
        }

        public ReplaceEngine Replace(Item originalItem)
        {
            if (!originalItem.DoesAllow(this,ModifyChecklist))
                throw new InvalidOperationException("Does not have permission to modify checklist");
            return new(this, originalItem);
        }

        public void Reset(Item item)
        {
            if (!Can(Set).On(item))
                throw new InvalidOperationException("Does not have permission to reset an Item");
            item.Reset();
        }

        public class ActionEngine
        {
            private readonly Person _person;
            private readonly Operation _operation;

            internal ActionEngine(Person person, Operation operation)
            {
                _person = person;
                _operation = operation;
            }

            public bool On(Item item) => item.DoesAllow(_person, _operation);

            internal bool To(Checklist checklist) => checklist.HasCreator(_person);
        }

        public class AssignEngine
        {
            private readonly Person _person;
            private readonly Item _item;

            internal AssignEngine(Person person, Item item)
            {
                _person = person;
                _item = item;
            }

            public void To(object value)
            {
                if (!_person.Can(Set).On(_item))
                    throw new InvalidOperationException("Does not have permission to set an Item");
                _item.Be(value);
            }
        }

        public class ReplaceEngine
        {
            private readonly Person _person;
            private readonly Item _originalItem;
            private Item _firstItem;
            private Item[] _items;

            internal ReplaceEngine(Person person, Item originalItem)
            {
                _person = person;
                _originalItem = originalItem;
            }

            public ReplaceEngine With(Item firstItem, params Item[] items)
            {
                _firstItem = firstItem;
                _items = items;
                return this;
            }

            public void In(Checklist checklist) {
                EnsureItemNotInTree(_originalItem, _firstItem);
                foreach (var item in _items) EnsureItemNotInTree(_originalItem, item);
                var newItem = _items.Length == 0 ? _firstItem : new GroupItem(_firstItem, _items);
                checklist.Replace(_originalItem, newItem);
            }

            private void EnsureItemNotInTree(Item originalItem, Item targetItem) {
                if (_originalItem == targetItem) return;
                if (targetItem.Contains(originalItem)) 
                    throw new InvalidOperationException("can't reinsert item being replaced");
            }
        }

        public class InsertEngine
        {
            private readonly Person _person;
            private readonly Item _item;
            private readonly List<Item> _items;
            private Item _originalItem;
            private Item _firstItem;

            internal InsertEngine(Person person, Item item, Item[] items)
            {
                _person = person;
                _item = item;
                _items = items.ToList();
            }

            public void In(Checklist checklist) => 
                _person.Replace(_originalItem).With(_firstItem, _items.ToArray()).In(checklist);

            public InsertEngine After(Item originalItem)
            {
                _originalItem = originalItem;
                _firstItem = originalItem;
                _items.Insert(0, _item);
                return this;
            }

            public InsertEngine Before(Item originalItem)
            {
                _originalItem = originalItem;
                _firstItem = _item;
                _items.Add(originalItem);
                return this;
            }
        }

        public class RemoveEngine
        {
            private readonly Item _item;

            public RemoveEngine(Item item)
            {
                _item = item;
            }

            public void From(Checklist checklist)
            {
                checklist.Remove(_item);
            }
        }

        public class AddingEngine
        {
            private readonly Person _addingPerson;
            private readonly Person _addedPerson;
            private Role _role;

            internal AddingEngine(Person addingPerson, Person addedPerson)
            {
                _addingPerson = addingPerson;
                _addedPerson = addedPerson;
            }

            public AddingEngine As(Role role)
            {
                _role = role;
                return this;
            }

            public void To(Item item)
            {
                if (!_addingPerson.Can(AddPerson).On(item))
                    throw new InvalidOperationException("Does not have permission to add new person");
                item.AddPerson(_addedPerson, _role);

            }
        }
    }
}