namespace Engine.Items
{
	public class BooleanItem : Item
	{
		private bool? hasSucceeded;
        public void Be(object value) => hasSucceeded = (bool)value;

        public void Reset() => hasSucceeded = null;

        public ItemStatus Status() => hasSucceeded switch
        {
            true => ItemStatus.Succeeded,
            false => ItemStatus.Failed,
            _ => ItemStatus.Unknown,
        };
    }
}