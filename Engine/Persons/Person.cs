using Engine.Items;

namespace Engine.Persons
{
    public class Person
    {
		public AddingPerson Add(Person owner)
		{
      return new AddingPerson(this, owner);
		}

    

		public ActionValidation Can(Operation view) => new ActionValidation(this, view);

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
				if(!_addingPerson.Can(Operation.AddPerson).On(item)) 
					throw new InvalidOperationException("Does not have permission to add new person");
				item.AddPerson(_addedPerson, _role);
				
			}
		}
	}
}