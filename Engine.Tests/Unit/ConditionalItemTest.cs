using Engine.Items;
using Engine.Persons;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
	public class ConditionalItemTest
	{
		private readonly static Person creator = new Person();
		[Fact]
		public void Boolean()
		{
			var baseItem = new BooleanItem();
			var successItem = new BooleanItem();
			var failItem = new BooleanItem();

			var compositeItem = new ConditionalItem(baseItem, successItem, failItem);
			var checklist = new Checklist( creator, compositeItem);

			Assert.Equal(InProgress, checklist.Status());
			creator.Sets(baseItem).To(true);
			Assert.Equal(InProgress, checklist.Status());
			creator.Sets(successItem).To(false);
			Assert.Equal(Failed, checklist.Status());
			creator.Sets(baseItem).To(false);
			Assert.Equal(InProgress, checklist.Status());
			creator.Sets(failItem).To(true);
			Assert.Equal(Succeeded, checklist.Status());
		}
        [Fact]
        public void BooleanWithFalse()
        {
            var baseItem = new BooleanItem();
            var failItem = new BooleanItem();

            var compositeItem = new ConditionalItem(baseItem, failItem : failItem);
            var checklist = new Checklist( creator, compositeItem);

            Assert.Equal(InProgress, checklist.Status());
            creator.Sets(baseItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
            creator.Sets(baseItem).To(false);
            Assert.Equal(InProgress, checklist.Status());
            creator.Sets(failItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
        }
        [Fact]
        public void BooleanWithTrue()
        {
            var baseItem = new BooleanItem();
            var successItem = new BooleanItem();

            var compositeItem = new ConditionalItem(baseItem, successItem: successItem);
            var checklist = new Checklist( creator, compositeItem);

            Assert.Equal(InProgress, checklist.Status());
            creator.Sets(baseItem).To(false);
            Assert.Equal(Failed, checklist.Status());
            creator.Sets(baseItem).To(true);
            Assert.Equal(InProgress, checklist.Status());
            creator.Sets(successItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
        }
        [Fact]
        public void MultipleChoice()
        {
            var baseItem = new BooleanItem();
            var successItem = new BooleanItem();
            var failitem = new BooleanItem();
            var compositeItem = new ConditionalItem(baseItem, successItem, failitem);
            var checklist = new Checklist( creator, compositeItem);
        }
    }
}
