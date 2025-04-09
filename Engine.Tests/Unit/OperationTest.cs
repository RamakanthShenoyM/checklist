using Engine.Items;
using Xunit;
using Engine.Persons;
using static Engine.Persons.Operation;
using static Engine.Persons.Role;
using static Engine.Items.ChecklistExtensions;
using Engine.Tests.Util;
using Xunit.Sdk;
using Xunit.Abstractions;
using static Engine.Items.ChecklistEventType;

namespace Engine.Tests.Unit
{
    public class OperationTest(ITestOutputHelper testOutput)
	{
		private static readonly Person Creator = new Person(0, 0);
		private static readonly Person Owner = new Person(3, 4);
		
		[Fact]
		public void SingleItem() {
			var checklist = Creator.Checklist(
				"Is India citizen?".TrueFalse(),
				"Is Kanataka Resident?".TrueFalse()
			);
			var item1 = checklist.I(0, 0);
			var item2 = checklist.I(0, 1);
			
			Assert.True(Creator.Can(Set).On(item1));
			Assert.False(Owner.Can(Set).On(item1));
			Creator.Add(Owner).As(Role.Owner).To(item1);
			Creator.Add(Owner).As(Role.Owner).To(item2);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
            Assert.Equal(2, history.Events(PersonAddEvent).Count);
            Assert.True(Owner.Can(Set).On(item1));
			Assert.False(Owner.Can(ModifyChecklist).On(item1));
		}
        [Fact]
        public void GroupItem()
        {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),

                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess: "Second success leg".TrueFalse(),
                        onFailure: "Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not(new BooleanItem("First Or of first failure leg")),
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            var item1 = checklist.I(0, 0);
            var item2 = checklist.I(0, 1);
            Creator.Add(Owner).As(Role.Owner).To(checklist);
            Creator.Add(Owner).As(Role.Owner).To(item2);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
        }


		[Fact]
		public void RemovePerson() {
			var checklist = Creator.Checklist(
				"Is India citizen?".TrueFalse(),
				"Is Kanataka Resident?".TrueFalse()
			);
			var item1 = checklist.I(0, 0);
			var item2 = checklist.I(0, 1);

			Creator.Add(Owner).As(Role.Owner).To(checklist);
			Assert.True(Owner.Can(Set).On(item1));
			Assert.True(Owner.Can(Set).On(item2));
			Creator.Remove(Owner).From(item1);
			Creator.Remove(Owner).From(item2);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
        }
	}
}
