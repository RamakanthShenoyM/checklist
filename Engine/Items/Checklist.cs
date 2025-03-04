using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Items
{
	public class Checklist
	{
		private readonly List<Item> _items;

		public Checklist(params Item[] items)
		{
            _items = items.ToList();
		}

        public void Add(params Item[] items) => _items.AddRange(items);

        public void Cancel(Item item) => _items.Remove(item);

        public ChecklistStatus Status()
		{
			if(_items.Count == 0 ) return ChecklistStatus.NotApplicable;
			var statuses = _items.Select(item => item.Status());
			if (statuses.All(status => status == ItemStatus.Succeeded))
				return ChecklistStatus.Succeeded;
			if (statuses.Any(status => status == ItemStatus.Failed))
				return ChecklistStatus.Failed;
			return ChecklistStatus.InProgress;
		}
	}
}
