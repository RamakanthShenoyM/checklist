using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Items
{
	public class Checklist
	{
		private readonly List<BooleanItem> _booleanItems;

		public Checklist(params BooleanItem[] booleanItems)
		{
			_booleanItems = booleanItems.ToList();
		}

        public void Add(params BooleanItem[] items) => _booleanItems.AddRange(items);

        public void Cancel(BooleanItem item) => _booleanItems.Remove(item);

        public ChecklistStatus Status()
		{
			var statuses = _booleanItems.Select(item => item.Status());
			if (statuses.All(status => status == ItemStatus.Succeeded))
				return ChecklistStatus.Succeeded;
			if (statuses.Any(status => status == ItemStatus.Failed))
				return ChecklistStatus.Failed;
			return ChecklistStatus.InProgress;
		}
	}
}
