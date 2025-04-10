using Engine.Items;
using Engine.Persons;
using System;
using Xunit;
using static Engine.Items.ChecklistStatus;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit
{
    public class NotItemTest
    {

        private static readonly Person Creator = new Person(0, 0);
		[Fact]
        public void NotBoolean() {
            var checklist = Creator.Checklist(
                Not( new BooleanItem("Is US citizen?") )
            );
            var booleanItem = (SimpleItem)checklist.P(0, 0);
            var notItem = checklist.P(0);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(booleanItem).To(true);
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(booleanItem).To(false);
            Assert.Equal(Succeeded, checklist.Status());
            Creator.Reset(booleanItem);
            Assert.Equal(InProgress, checklist.Status());
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
        }

        [Fact]
        public void NotMultipleChoice()
        {
            var checklist = Creator.Checklist(
                Not( "Which country?".Choices("India","Srilanka") )
            );
            var multipleChoiceItem = (SimpleItem)checklist.P(0, 0);
            var notItem = checklist.P(0);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(multipleChoiceItem).To("India");
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(multipleChoiceItem).To("Srilanka");
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(multipleChoiceItem).To("Bangladesh");
            Assert.Equal(Succeeded, checklist.Status());
            Creator.Reset(multipleChoiceItem);
            Assert.Equal(InProgress, checklist.Status());
        }
    }
}
