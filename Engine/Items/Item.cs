using Engine.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Items
{
	public abstract class Item
	{
		private readonly List<Creator> _persons = new List<Creator>();
		internal abstract ItemStatus Status();
		public abstract void Be(object value);
		public abstract void Reset();

		internal void AddPerson(Creator person) => _persons.Add(person);

		internal bool HasPerson(Creator person) => _persons.Contains(person);
	}

	public static class ItemExtensions
	{
		public static NotItem Not(this Item item) => new NotItem(item);
		public static OrItem Or(this Item item1, Item item2) => new OrItem(item1, item2);

	}
}
