using Engine.Items;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
	public class CompositeItemTest
	{
		[Fact]
		public void Boolean()
		{
			var baseItem = new BooleanItem();
			var successItem = new BooleanItem();
			var failItem = new BooleanItem();

			var compositeItem = new CompositeItem(baseItem, successItem, failItem);
			var checklist = new Checklist(compositeItem);

			Assert.Equal(InProgress, checklist.Status());
			baseItem.Be(true);
			Assert.Equal(InProgress, checklist.Status());
			successItem.Be(false);
			Assert.Equal(Failed, checklist.Status());
			baseItem.Be(false);
			Assert.Equal(InProgress, checklist.Status());
			failItem.Be(true);
			Assert.Equal(Succeeded, checklist.Status());
		}
	}
}
