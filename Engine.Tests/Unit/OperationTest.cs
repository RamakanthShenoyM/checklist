using Engine.Items;
using Xunit;
using Engine.Persons;
using static Engine.Persons.Operation;
using static Engine.Persons.Role;

namespace Engine.Tests.Unit
{
    public class OperationTest
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

			Assert.True(Owner.Can(Set).On(item1));
			Assert.False(Owner.Can(ModifyChecklist).On(item1));
		}
		[Fact]
		public void GroupItem() {
			var checklist = Creator.Checklist(
				"Is India citizen?".TrueFalse(),
				"Is Kanataka Resident?".TrueFalse()
			);
			var item1 = checklist.I(0, 0);
			var item2 = checklist.I(0, 1);
			
			Assert.True(Creator.Can(View).On(item1));
			Assert.False(Owner.Can(View).On(item1));

			Creator.Add(Owner).As(Role.Owner).To(checklist);
			Assert.True(Owner.Can(Set).On(item1));
			Assert.True(Owner.Can(Set).On(item2));
		}
	}
}
