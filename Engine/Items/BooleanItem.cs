
using System.Diagnostics.Contracts;
namespace Engine.Items
{
	public class BooleanItem
	{
		private bool? hasSucceeded;

		public void Be(bool value)
		{
			 hasSucceeded = value;
		}

		public void Reset()
		{
			hasSucceeded = null;
		}

		public ItemStatus Status()
		{
			switch (hasSucceeded) {
				case true:
					return ItemStatus.Succeeded;
				case false:
					return ItemStatus.Failed;
				default:
					return ItemStatus.InProgress;
			}
		}
	}
}