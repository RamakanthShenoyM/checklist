using Engine.Persons;

namespace Engine.Items
{
	public abstract class Item
	{
		private readonly Dictionary<Person, List<Operation>> _operations = [];
		internal abstract ItemStatus Status();
		public abstract void Be(object value);
		public abstract void Reset();
		internal void AddPerson(Person person, Role role) => _operations[person] = role.Operations;
		internal bool HasPerson(Person person) => _operations.Keys.Contains(person);
	}

	public static class ItemExtensions
	{
		public static NotItem Not(this Item item) => new NotItem(item);
		public static OrItem Or(this Item item1, Item item2) => new OrItem(item1, item2);

	}
}
