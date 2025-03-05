using Engine.Items;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
	public class CompositeItem(Item baseItem, Item? successItem = null, Item? failItem = null) : Item
	{

        public void Be(object value) => baseItem.Be(value);

        public void Reset()
		{
			baseItem.Reset();
		}

		public ItemStatus Status()
		{
			if(baseItem.Status() == Succeeded) return successItem?.Status() ?? Succeeded;
			if(baseItem.Status() == Failed) return failItem?.Status() ?? Failed;
			return Unknown;
		}
	}
}