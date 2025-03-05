using Engine.Items;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
	public class CompositeItem(BooleanItem baseItem, BooleanItem successItem, BooleanItem failItem) : Item
	{
		public void Be(object value)
		{
			baseItem.Be(value);
		}

		public void Reset()
		{
			baseItem.Reset();
			successItem.Reset();
			failItem.Reset();
		}

		public ItemStatus Status()
		{
			if(baseItem.Status() == Succeeded) return successItem.Status();
			if(baseItem.Status() == Failed) return failItem.Status();
			return Unknown;
		}
	}
}