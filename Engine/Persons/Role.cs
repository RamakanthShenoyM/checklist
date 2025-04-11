using static Engine.Persons.Operation;
namespace Engine.Persons
{
    public class Role(List<Operation> operations)
    {
        public static readonly Role Creator = new Role(new List<Operation> {
        View,
        ModifyChecklist,
        Set,
        SetRole});

		public static readonly Role Owner = new Role(new List<Operation> {
				View,
				Set,
				SetRole});

		public static readonly Role Viewer = new Role(new List<Operation> {
				View
		});

		internal List<Operation> Operations { get; } = operations;

	}
	public enum Operation
	{
		View,
		ModifyChecklist,  // Insert after; Insert before; Replace; Remove
		Set,			  // Set a value or Reset
		SetRole			  // Add a Person with a Role; Remove a Person
	}
}
