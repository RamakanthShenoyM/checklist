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
			var item1 = checklist.P(0, 0);
			var item2 = checklist.P(0, 1);
			
			Assert.True(Creator.Can(Set).On(item1));
			Assert.False(Owner.Can(Set).On(item1));

			Creator.Add(Owner).As(Role.Owner).To(item1);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
            Assert.Single(history.Events(PersonAddEvent));
            Assert.True(Owner.Can(Set).On(item1));
			Assert.False(Owner.Can(ModifyChecklist).On(item1));
		}
       

		[Fact]
		public void RemovePerson() {
			var checklist = Creator.Checklist(
				"Is India citizen?".TrueFalse(),
				"Is Kanataka Resident?".TrueFalse()
			);
			var item1 = checklist.P(0, 0);
			var item2 = checklist.P(0, 1);

			Creator.Add(Owner).As(Role.Owner).To(checklist);
			Assert.True(Owner.Can(Set).On(item1));
			Assert.True(Owner.Can(Set).On(item2));
			Creator.Remove(Owner).From(item1);
            Assert.False(Owner.Can(Set).On(item1));
            Assert.True(Owner.Can(Set).On(item2));
        }
	}
}
