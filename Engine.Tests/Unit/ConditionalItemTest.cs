using Engine.Items;
using Engine.Persons;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
	public class ConditionalItemTest
	{
		private static readonly Person Creator = new();
		[Fact]
		public void Boolean()
		{
			var baseItem = new BooleanItem();
			var successItem = new BooleanItem();
			var failItem = new BooleanItem();

			var compositeItem = new ConditionalItem(baseItem, successItem, failItem);
			var checklist = new Checklist( Creator, compositeItem);

			Assert.Equal(InProgress, checklist.Status());
			Creator.Sets(baseItem).To(true);
			Assert.Equal(InProgress, checklist.Status());
			Creator.Sets(successItem).To(false);
			Assert.Equal(Failed, checklist.Status());
			Creator.Sets(baseItem).To(false);
			Assert.Equal(InProgress, checklist.Status());
			Creator.Sets(failItem).To(true);
			Assert.Equal(Succeeded, checklist.Status());
			var output = new PrettyPrint(checklist).Result();
			// Assert.Equal("", output);
		}
        [Fact]
        public void BooleanWithFalse()
        {
            var baseItem = new BooleanItem();
            var failItem = new BooleanItem();

            var compositeItem = new ConditionalItem(baseItem, failItem : failItem);
            var checklist = new Checklist( Creator, compositeItem);

            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(baseItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
            Creator.Sets(baseItem).To(false);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(failItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
        }
        [Fact]
        public void BooleanWithTrue()
        {
            var baseItem = new BooleanItem();
            var successItem = new BooleanItem();

            var compositeItem = new ConditionalItem(baseItem, successItem: successItem);
            var checklist = new Checklist( Creator, compositeItem);

            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(baseItem).To(false);
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(baseItem).To(true);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(successItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
        }
    }
}
