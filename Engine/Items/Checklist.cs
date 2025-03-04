using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Items
{
	public class Checklist
	{
		private readonly BooleanItem _booleanItem;

		public Checklist(BooleanItem booleanItem)
		{
			_booleanItem = booleanItem;
		}

		public ChecklistStatus Status()
		{
			switch (_booleanItem.Status())
			{
				case ItemStatus.Succeeded:
					return ChecklistStatus.Succeeded;
				case ItemStatus.Failed:
					return ChecklistStatus.Failed;
				case ItemStatus.InProgress:
					return ChecklistStatus.InProgress;
				default:
					throw new InvalidOperationException("Invalid ItemStatus");
			}
		}
	}
}
