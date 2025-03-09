using static Engine.Persons.Operation;
namespace Engine.Persons
{
    public class Role(List<Operation> operations)
    {
        public static readonly Role Creator = new Role(new List<Operation> {
        View,
        ModifyChecklist,
        Set,
        AddPerson,
        RemovePerson});

		public static readonly Role Owner = new Role(new List<Operation> {
				View,
				Set,
				AddPerson,
				RemovePerson});

		public static readonly Role Viewer = new Role(new List<Operation> {
				View
		});

		internal List<Operation> Operations { get; } = operations;

	}
	public enum Operation
	{
		View,
		ModifyChecklist,
		Set,
		AddPerson,
		RemovePerson
	}
}
