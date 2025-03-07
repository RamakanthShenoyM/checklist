using Engine.Persons;

namespace Engine.Items
{
	public abstract class Item
	{
		protected readonly Dictionary<Person, List<Operation>> Operations = [];
		internal abstract ItemStatus Status();
		internal abstract void Be(object value);
		internal abstract void Reset();
		internal virtual void AddPerson(Person person, Role role) => Operations[person] = role.Operations;
		internal bool HasPerson(Person person) => Operations.Keys.Contains(person);
		internal bool DoesAllow(Person person, Operation operation) => 
			Operations.ContainsKey(person) && Operations[person].Contains(operation);
		internal virtual bool Contains(Item desiredItem) => this == desiredItem;
		internal abstract void Accept(ChecklistVisitor visitor);
	}

	public static class ItemExtensions
	{
		public static NotItem Not(this Item item) => new NotItem(item);
		public static OrItem Or(this Item item1, Item item2) => new OrItem(item1, item2);

	}
}
