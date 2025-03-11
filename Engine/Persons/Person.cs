using Engine.Items;
using static Engine.Persons.Operation;

namespace Engine.Persons
{
    public class Person
    {
        public AddingPerson Add(Person owner)
        {
            return new AddingPerson(this, owner);
        }

        public ActionValidation Can(Operation view) => new ActionValidation(this, view);

        public void Reset(Item item)
        {
            if (!this.Can(Set).On(item))
                throw new InvalidOperationException("Does not have permission to reset an Item");
            item.Reset();

        }

        public AssignToItem Sets(Item item)
        {
            return new AssignToItem(this, item);
        }

        public class ActionValidation
        {
            private readonly Person _person;
            private readonly Operation _operation;

            internal ActionValidation(Person person, Operation operation)
            {
                this._person = person;
                this._operation = operation;
            }

            public bool On(Item item) => item.DoesAllow(_person, _operation);

            internal bool To(Checklist checklist) => checklist.HasCreator(_person);
        }

        public class AddingPerson
        {
            private readonly Person _addingPerson;
            private readonly Person _addedPerson;
            private Role _role;

            internal AddingPerson(Person addingPerson, Person addedPerson)
            {
                _addingPerson = addingPerson;
                _addedPerson = addedPerson;
            }

            public AddingPerson As(Role role)
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

        public class AssignToItem
        {
            private readonly Person _person;
            private readonly Item _item;

            internal AssignToItem(Person person, Item item)
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

        public ReplaceEngine Replace1(Item originalItem)
        {
            if (!originalItem.DoesAllow(this,ModifyChecklist))
                throw new InvalidOperationException("Does not have permission to modify checklist");
            return new ReplaceEngine(this, originalItem);
        }

        public class ReplaceEngine
        {
            private readonly Person _person;
            private readonly Item _originalItem;
            private Item _newItem;

            internal ReplaceEngine(Person person, Item originalItem)
            {
                _person = person;
                _originalItem = originalItem;
            }

            public ReplaceEngine With(Item firstItem, params Item[] items)
            {
                _newItem = items.Length==0 ? firstItem : new GroupItem(firstItem, items);
                return this;
            }

            public void In(Checklist checklist) => checklist.Replace(_originalItem, _newItem);
        }

        public class InsertEngine
        {
            private readonly Person _person;
            private readonly Item _item;
            private Item _newItem;
            private readonly List<Item> _items;
            private Item _originalItem;

            internal InsertEngine(Person person, Item item, Item[] items)
            {
                _person = person;
                _item = item;
                _items = items.ToList();
            }

            public void In(Checklist checklist) => _person.Replace1(_originalItem).With(_newItem).In(checklist);

            public InsertEngine After(Item originalItem)
            {
                _items.Insert(0,_item);
                _newItem=new GroupItem(originalItem, _items.ToArray());
                _originalItem = originalItem;
                return this;
            }

            public InsertEngine Before(Item originalItem)
            {
                _items.Add(originalItem);
                _newItem = new GroupItem(_item, _items.ToArray());
                _originalItem = originalItem;
                return this;
            }
        }

        public InsertEngine Insert1(Item firstItem, params Item[] items) => new InsertEngine(this, firstItem,items);

        public RemoveEngine Remove(Item item)
        {
            if (!this.Can(ModifyChecklist).On(item))
                throw new InvalidOperationException("Does not have permission to modify checklist");

            return new RemoveEngine(item);
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
    }
}