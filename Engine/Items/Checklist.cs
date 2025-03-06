using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Persons;
using System.Threading.Tasks;
using static Engine.Persons.Role;


namespace Engine.Items
{
	public class Checklist
	{
		private readonly List<Item> _items;
		private readonly Person _creator;

		public Checklist(Person creator, Item firstItem, params Item[] items)
		{
			_items = items.ToList();
			_items.Insert(0, firstItem);
			_creator = creator;
			_items.ForEach(item => item.AddPerson(_creator, Creator));	
		}

        public void Add(params Item[] items)
        {
            items.ToList().ForEach(item => item.AddPerson(_creator, Creator));
            _items.AddRange(items);
        }

        public void Cancel(Item item) => _items.Remove(item);

		public List<Item> Failures() => _items.FindAll(item => item.Status() == ItemStatus.Failed);
		public ChecklistStatus Status()
		{
			if (_items.Count == 0) return ChecklistStatus.NotApplicable;
			var statuses = _items.Select(item => item.Status());
			if (statuses.All(status => status == ItemStatus.Succeeded))
				return ChecklistStatus.Succeeded;
			if (statuses.Any(status => status == ItemStatus.Failed))
				return ChecklistStatus.Failed;
			return ChecklistStatus.InProgress;
		}

		public List<Item> Successes() => _items.FindAll(item => item.Status() == ItemStatus.Succeeded);

		public List<Item> Unknowns() => _items.FindAll(item => item.Status() == ItemStatus.Unknown);
		internal bool Contains(Item desiredItem) => _items.Any(item => item.Contains(desiredItem));

        internal bool HasCreator(Person person) => person == _creator;
    }
}
