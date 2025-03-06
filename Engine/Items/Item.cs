using Engine.Persons;

namespace Engine.Items
{
	public abstract class Item
	{
		private readonly Dictionary<Person, List<Operation>> _operations = [];
		internal abstract ItemStatus Status();
		internal abstract void Be(object value);
		internal abstract void Reset();
		internal virtual void AddPerson(Person person, Role role) => _operations[person] = role.Operations;
		internal bool HasPerson(Person person) => _operations.Keys.Contains(person);
		internal bool DoesAllow(Person person, Operation operation) => 
			_operations.ContainsKey(person) && _operations[person].Contains(operation);
		internal virtual bool Contains(Item desiredItem) => this == desiredItem;

	}

	public static class ItemExtensions
	{
		public static NotItem Not(this Item item) => new NotItem(item);
		public static OrItem Or(this Item item1, Item item2) => new OrItem(item1, item2);

	}
}
