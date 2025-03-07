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

        public AdditionValidation Add(Item firstItem, params Item[] items)
        {
            return new AdditionValidation(this, firstItem, items);
        }

        public ActionValidation Can(Operation view) => new ActionValidation(this, view);

        public CancelValidation Cancel(Item item) => new CancelValidation(this, item);

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

        public class CancelValidation
        {
            private readonly Person _person;
            private readonly Item _item;

            internal CancelValidation(Person person, Item item)
            {
                _person = person;
                _item = item;
            }

            public void In(Checklist checklist)
            {
                if (!checklist.Contains(_item))
                    throw new InvalidOperationException("Item is not present in the Checklist");
                if (!_person.Can(Operation.Cancel).On(_item))
                    throw new InvalidOperationException("Does not have permission to modify an Item");
                checklist.Cancel(_item);
            }
        }

        public class AdditionValidation
        {
            private readonly Person _person;
            private readonly List<Item> _items;

            internal AdditionValidation(Person person, Item firstItem, Item[] items)
            {
                _person = person;
                _items = items.ToList();
                _items.Insert(0, firstItem);
            }

            public void In(Checklist checklist)
            {
                if (!_person.Can(AddItem).To(checklist))
                    throw new InvalidOperationException("Does not have permission to Add to the checklist");
                checklist.Add(_items.ToArray<Item>());
            }
        }
    }
}