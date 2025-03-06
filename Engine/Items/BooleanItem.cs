using Engine.Persons;

namespace Engine.Items
{
	public class BooleanItem : Item
	{
		private bool? hasSucceeded;
        internal override void Be(object value) => hasSucceeded = (bool)value;

        internal override void Reset() => hasSucceeded = null;

		

		internal override ItemStatus Status() => hasSucceeded switch
        {
            true => ItemStatus.Succeeded,
            false => ItemStatus.Failed,
            _ => ItemStatus.Unknown,
        };
    }
}