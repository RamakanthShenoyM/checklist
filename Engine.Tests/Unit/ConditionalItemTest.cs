using Engine.Items;
using Engine.Persons;
using System;
using Xunit;
using Xunit.Abstractions;
using static Engine.Items.ChecklistStatus;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit
{
	public class ConditionalItemTest(ITestOutputHelper testOutput)
	{
		private static readonly Person Creator = new(0, 0);
		
		[Fact]
		public void Boolean()
		{
			var checklist = Creator.Checklist(
				Conditional(
				"Is US citizen?".TrueFalse(),
				"Is Nevada resident?".TrueFalse(),
				"Is Canadian citizen?".TrueFalse()
			));
			var baseItem = checklist.I(0, 0);
			var successItem = checklist.I(0, 1);
			var failItem = checklist.I(0, 2);

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

            var compositeItem = new ConditionalItem(baseItem, onFail: failItem);
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

            var compositeItem = new ConditionalItem(baseItem, onSuccess: successItem);
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
        public void ConditionalWithConditional() {
	        var checklist = Creator.Checklist(
		        "First simple item".TrueFalse(),
		        Conditional(
			        condition: "First condition".TrueFalse(),
			        onSuccess: Conditional(
				        condition: "Second condition".TrueFalse(),
				        onSuccess: "Second success leg".TrueFalse(),
				        onFailure: "Second failure leg".TrueFalse()
			        ),
			        onFailure: Not(
				        Or(
					        "First Or of first failure leg".TrueFalse(),
					        "Second Or of first failure leg".TrueFalse()
				        )
			        )
		        ),
		        "Last simple item".TrueFalse()
	        );
	        testOutput.WriteLine(checklist.ToString());
        }
    }
}
