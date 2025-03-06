using static Engine.Persons.Operation;
namespace Engine.Persons
{
    public class Role(List<Operation> operations)
    {
        public static readonly Role Creator = new Role(new List<Operation> {
        View,
        AddItem,
        Cancel,
        DeleteItem,
        Set,
        Reset,
        AddPerson,
        RemovePerson});

        internal List<Operation> Operations { get; } = operations;

    }
}
