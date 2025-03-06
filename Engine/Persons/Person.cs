namespace Engine.Persons
{
    public class Person
    {
        public ActionValidation Can(Operation view)
        {
            return new ActionValidation(this, view);
        }

        public class ActionValidation
        {
            private readonly Person _person;
            private readonly Operation _permission;

            internal ActionValidation(Person person, Operation permission)
            {
                this._person = person;
                this._permission = permission;
            }
        }
    }
}