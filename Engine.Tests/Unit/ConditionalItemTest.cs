using Engine.Items;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
	public class ConditionalItemTest
	{
		[Fact]
		public void Boolean()
		{
			var baseItem = new BooleanItem();
			var successItem = new BooleanItem();
			var failItem = new BooleanItem();

			var compositeItem = new ConditionalItem(baseItem, successItem, failItem);
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
        [Fact]
        public void BooleanWithFalse()
        {
            var baseItem = new BooleanItem();
            var failItem = new BooleanItem();

            var compositeItem = new ConditionalItem(baseItem, failItem : failItem);
            var checklist = new Checklist(compositeItem);

            Assert.Equal(InProgress, checklist.Status());
            baseItem.Be(true);
            Assert.Equal(Succeeded, checklist.Status());
            baseItem.Be(false);
            Assert.Equal(InProgress, checklist.Status());
            failItem.Be(true);
            Assert.Equal(Succeeded, checklist.Status());
        }
        [Fact]
        public void BooleanWithTrue()
        {
            var baseItem = new BooleanItem();
            var successItem = new BooleanItem();

            var compositeItem = new ConditionalItem(baseItem, successItem: successItem);
            var checklist = new Checklist(compositeItem);

            Assert.Equal(InProgress, checklist.Status());
            baseItem.Be(false);
            Assert.Equal(Failed, checklist.Status());
            baseItem.Be(true);
            Assert.Equal(InProgress, checklist.Status());
            successItem.Be(true);
            Assert.Equal(Succeeded, checklist.Status());
        }
        [Fact]
        public void MultipleChoice()
        {
            var baseItem = new BooleanItem();
            var successItem = new BooleanItem();
            var failitem = new BooleanItem();
            var compositeItem = new ConditionalItem(baseItem, successItem, failitem);
            var checklist = new Checklist(compositeItem);
        }
    }
}
