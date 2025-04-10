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
using static Engine.Tests.Unit.CarpetColor;

namespace Engine.Tests.Unit
{
    public class OperationTest(ITestOutputHelper testOutput)
	{
		private static readonly Person Creator = new Person(0, 0);
		private static readonly Person Owner = new Person(3, 4);
        private static readonly Person Viewer = new Person(10, 20);
		
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
            var item1 = checklist.P(0, 0);
            var item2 = checklist.P(0, 1);
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
			var item1 = checklist.P(0, 0);
			var item2 = checklist.P(0, 1);

			Creator.Add(Owner).As(Role.Owner).To(checklist);
			Assert.True(Owner.Can(Set).On(item1));
			Assert.True(Owner.Can(Set).On(item2));

			Creator.Remove(Owner).From(item1);
			Creator.Remove(Owner).From(item2);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
        }

        [Fact]
        public void MultipleChoiceItemAuthority()
        {
            var checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet)
            );
            var item = checklist.P(0);
			Creator.Add(Owner).As(Role.Owner).To(item);
            Assert.True(Owner.Can(Set).On(item));
            Assert.False(Owner.Can(ModifyChecklist).On(item));

			Creator.Remove(Owner).From(item);
            Assert.False(Owner.Can(Set).On(item));
            Assert.False(Owner.Can(ModifyChecklist).On(item));

			Creator.Add(Viewer).As(Role.Viewer).To(item);
			Assert.False(Viewer.Can(Set).On(item));
            Creator.Add(Viewer).As(Role.Owner).To(item);
            Assert.True(Viewer.Can(Set).On(item));
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);

        }

        [Fact]
        public void ConditionalItemAuthority()
        {
            var checklist = Creator.Checklist(
                Conditional(
                    condition: "Is US citizen?".TrueFalse(),
                    onFailure: "Is Canadian citizen?".TrueFalse()
                ));
            var baseItem = checklist.P(0, 0);
            var failItem = checklist.P(0, 2);

            Creator.Add(Owner).As(Role.Owner).To(baseItem);
            Assert.False(Owner.Can(Set).On(failItem));
            Assert.True(Owner.Can(Set).On(baseItem));

            Creator.Remove(Owner).From(baseItem);
            Assert.False(Owner.Can(Set).On(baseItem));

            Creator.Add(Owner).As(Role.Owner).To(checklist);
            Assert.True(Owner.Can(Set).On(baseItem));
            Assert.True(Owner.Can(Set).On(failItem));

        }

        [Fact]
        public void NotItemAuthority()
        {
            var checklist = Creator.Checklist(
                Not("Is US citizen?".TrueFalse())
            );
            var item = checklist.P(0, 0);
            Creator.Add(Owner).As(Role.Owner).To(item);
            Assert.True(Owner.Can(Set).On(item));

            Creator.Remove(Owner).From(item);
            Assert.False(Owner.Can(Set).On(item));
        }

        [Fact]
        public void OrItemAuthority()
        {
            var checklist= Creator.Checklist(
                Or(
                    "Is US citizen?".TrueFalse(),
                    "Is Canadian citizen?".TrueFalse()
                )
            );
            var item = checklist.P(0, 0);
            Creator.Add(Owner).As(Role.Owner).To(item);
            Assert.True(Owner.Can(Set).On(item));

            Creator.Remove(Owner).From(item);
            Assert.False(Owner.Can(Set).On(item));

            Creator.Add(Owner).As(Role.Owner).To(checklist);
            Assert.True(Owner.Can(Set).On(item));
        }

        [Fact]
        public void ComplexChecklistAuthority()
        {

            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse(),
                "Is Canadian citizen?".TrueFalse(),
                Conditional(
                    condition: "Is US citizen?".TrueFalse(),
                    onFailure: "Is Canadian citizen?".TrueFalse()
                ),
                Not("Is US citizen?".TrueFalse()),
                Or(
                    "Is US citizen?".TrueFalse(),
                    Conditional(
                        condition: "Is US citizen?".TrueFalse(),
                        onFailure: "Is Canadian citizen?".TrueFalse()
                    )
                )
            );
            var item1 = checklist.P(0, 0);
            var item2 = checklist.P(0, 1);
            var item3 = checklist.P(0, 2);
            var item4 = checklist.P(0, 3);
            var item5 = checklist.P(0, 4);
            var item31 = checklist.P(0, 2, 0);
            var item51 = checklist.P(0, 4, 1);
            Creator.Add(Owner).As(Role.Owner).To(item1);
            Assert.True(Owner.Can(Set).On(item1));
            Assert.False(Owner.Can(Set).On(item2));
            Assert.False(Owner.Can(Set).On(item3));
            Assert.False(Owner.Can(Set).On(item4));
            Assert.False(Owner.Can(Set).On(item5));

            Creator.Add(Owner).As(Role.Owner).To(item3);
            Assert.True(Owner.Can(Set).On(item3));
            Assert.True(Owner.Can(Set).On(item31));

            Creator.Remove(Owner).From(item3);
            Assert.False(Owner.Can(Set).On(item3));

            Creator.Add(Owner).As(Role.Owner).To(checklist);
            Assert.True(Owner.Can(Set).On(item1));

            Creator.Remove(Owner).From(item51);
            Assert.False(Owner.Can(Set).On(item51));
            Assert.True(Owner.Can(Set).On(item5));

            Owner.Add(Viewer).As(Role.Viewer).To(item4);
            Assert.False(Viewer.Can(Set).On(item4));
            Assert.False(Viewer.Can(Set).On(item51));

        }
    }
}
