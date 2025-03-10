using Engine.Items;
using Engine.Persons;
using System;
using Xunit;
using Xunit.Abstractions;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
	public class ConditionalItemTest(ITestOutputHelper testOutput)
	{
		private static readonly Person Creator = new();
		
		[Fact]
		public void Boolean()
		{
			var baseItem = new BooleanItem("Is US citizen?");
			var successItem = new BooleanItem("Is US citizen?");
			var failItem = new BooleanItem("Is US citizen?");

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
			testOutput.WriteLine(output);
		}
        [Fact]
        public void BooleanWithFalse()
        {
            var baseItem = new BooleanItem("Is US citizen?");
            var failItem = new BooleanItem("Is US citizen?");

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
            var baseItem = new BooleanItem("Is US citizen?");
            var successItem = new BooleanItem("Is US citizen?");

            var compositeItem = new ConditionalItem(baseItem, successItem: successItem);
            var checklist = new Checklist( Creator, compositeItem);

            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(baseItem).To(false);
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(baseItem).To(true);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(successItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
            Assert.Throws<InvalidOperationException>(() => Creator.Sets(compositeItem).To(true));
        }
        
        [Fact]
        public void ConditionalWithConditional()
        {
	        var baseItem1 = new BooleanItem("First condition");
	        var baseItem2 = new BooleanItem("Second condition");
	        var successItem2 = new BooleanItem("Second success leg");
	        var failItem2 = new BooleanItem("Second failure leg");
	        var successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
	        var failItem1 = new BooleanItem("First failure leg");

	        var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
	        var checklist = new Checklist( Creator, compositeItem);
	        testOutput.WriteLine(checklist.ToString());
        }
    }
}
