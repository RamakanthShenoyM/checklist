using CommonUtilities.Util;
using Engine.Items;
using static Engine.Items.ChecklistEventType;
using static Engine.Persons.Operation;

namespace Engine.Persons
{
    public class Person
    {
        internal readonly int _organizationId;
        internal readonly int _personId;

        public Person(int organizationId, int personId)
        {
            _organizationId = organizationId;
            _personId = personId;
        }


        public AddingEngine Add(Person owner) => new(this, owner);

        public ActionEngine Can(Operation view) => new(this, view);

        public AssignEngine Sets(SimpleItem item)
        {
            return new AssignEngine(this, item);

        }

        public InsertEngine Insert(Item firstItem, params Item[] items) => 
            new(this, firstItem,items);

        public RemoveEngine Remove(Item item)
        {
            if (!Can(ModifyChecklist).On(item))
                throw new InvalidOperationException("Does not have permission to modify checklist");
            return new(item);
        }
        public RemovePersonEngine Remove(Person person) => new(this,person);

        public ReplaceEngine Replace(Item originalItem)
        {
            if (!originalItem.DoesAllow(this,ModifyChecklist))
                throw new InvalidOperationException("Does not have permission to modify checklist");
            return new(this, originalItem);
        }

        public void Reset(SimpleItem item)
        {
            if (!Can(Set).On(item))
                throw new InvalidOperationException("Does not have permission to reset an Item");
            item.Reset();
            item.History().Add(ResetValueEvent, $"<{this}> reset item <{item}>");
        }
        public override string ToString() => $"org <{_organizationId}> person <{_personId}>";

        public override bool Equals(object? obj) => this == obj || obj is Person other && this.Equals(other);

        private bool Equals(Person other) =>
            this._organizationId==other._organizationId && this._personId==other._personId;

        public override int GetHashCode() => HashCode.Combine(_organizationId, _personId);

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
            private readonly SimpleItem _item;

            internal AssignEngine(Person person, SimpleItem item)

            {
                _person = person;
                _item = item;
            }

            public void To(object value)
            {
                if (!_person.Can(Set).On(_item))
                    throw new InvalidOperationException("Does not have permission to set an Item");
                try
                {
                    _item.Be(value);
                }
                catch (InvalidOperationException)
                {
                    _item.History().Add(InvalidValueEvent,
                        $"<{_person}> attempted to set item <{_item}> to invalid <{value}>");
                    throw;
                }
                catch (ArgumentNullException)
                {
                    _item.History().Add(InvalidValueEvent,
                        $"<{_person}> attempted to set item <{_item}> to null");
                    throw;
                }

                _item.History().Add(SetValueEvent, $"<{_person}> set item <{_item}> to <{value}>");
            }
        }

        public class ReplaceEngine
        {
            private readonly Person _person;
            private readonly Item _originalItem;
            private Item? _firstItem;
            private Item[] _items = [];

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

            public void In(Checklist checklist)
            {
                EnsureItemNotInTree(
                    _originalItem,
                    _firstItem ?? throw new InvalidOperationException("Improper DSL construction; missing first replacement Item")
                    );
                foreach (var item in _items) EnsureItemNotInTree(_originalItem, item);
                var newItem = _items.Length == 0 ? _firstItem : new GroupItem(_firstItem, _items);
                checklist.Replace(_originalItem, newItem);
                newItem.History().Add(ReplaceItemEvent, $"Item <{_originalItem}> at Position  <{_originalItem.Position()}> replaced with <{newItem}>  in checklist <{checklist}>");
            }

            private void EnsureItemNotInTree(Item originalItem, Item targetItem)
            {
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
            private Item? _originalItem;
            private Item? _firstItem;

            internal InsertEngine(Person person, Item item, Item[] items)
            {
                _person = person;
                _item = item;
                _items = items.ToList();
            }

            public void In(Checklist checklist) => _person
                .Replace(_originalItem ?? throw new InvalidOperationException("Improper DSL construction; missing 'After' clause"))
                .With(_firstItem ?? throw new InvalidOperationException("Improper DSL construction; missing Item to insert"), _items.ToArray())
                .In(checklist);

            public InsertEngine After(Item originalItem)
            {
                _originalItem = originalItem;
                _firstItem = originalItem;
                _items.Insert(0, _item);
                _originalItem.History().Add(InsertItemEvent, $"Item <{_item}> inserted after <{originalItem}> at Position <{originalItem.Position()}>");
                return this;
            }

            public InsertEngine Before(Item originalItem)
            {
                _originalItem = originalItem;
                _firstItem = _item;
                _items.Add(originalItem);
                _originalItem.History().Add(InsertItemEvent, $"Item <{_item}> inserted before <{originalItem}> at Position <{originalItem.Position()}>");
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
                _item.History().Add(RemoveItemEvent, $"Item <{_item}> at Position <{_item.Position()}> Removed from checklist <{checklist}> ");
            }
        }

        public class AddingEngine
        {
            private readonly Person _addingPerson;
            private readonly Person _addedPerson;
            private Role? _role;
            private readonly History _history = new([]);

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
                if (!_addingPerson.Can(SetRole).On(item))
                    throw new InvalidOperationException("Does not have permission to add new person");
                item.AddPerson(_addedPerson, _role);
                item.History().Add(PersonAddEvent, $"Person<{_addingPerson}> added <{_addedPerson}> To <{item}>");

            } 
            public void To(Checklist checklist) => To(checklist._item);
        }
        public class RemovePersonEngine
        {
            private readonly Person _removingPerson;
            private readonly Person _removedPerson;
            private readonly History _history = new([]);

            internal RemovePersonEngine(Person removingPerson, Person removedPerson)
            {
                _removingPerson = removingPerson;
                _removedPerson = removedPerson;
            }

            public void From(Item item)
            {
                if (!_removingPerson.Can(SetRole).On(item))
                    throw new InvalidOperationException("Does not have permission to remove person");
                item.RemovePerson(_removedPerson);
                item.History().Add(PersonRemovedEvent, $"Person<{_removingPerson}> removed <{_removedPerson}> From <{item}>");
                
            } 
            public void From(Checklist checklist) => From(checklist._item);
        }
    }
}