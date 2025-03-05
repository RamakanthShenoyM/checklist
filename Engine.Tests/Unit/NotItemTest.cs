using Engine.Items;
using Engine.Persons;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
    public class NotItemTest
    {

        private readonly static Creator creator = new Creator();
		[Fact]
        public void NotBoolean()
        {
            var booleanItem = new BooleanItem();
            var notItem = booleanItem.Not();
            var checklist = new Checklist( creator, notItem);
            Assert.Equal(InProgress, checklist.Status());
            booleanItem.Be(true);
            Assert.Equal(Failed, checklist.Status());
            booleanItem.Be(false);
            Assert.Equal(Succeeded, checklist.Status());
            booleanItem.Reset();
            Assert.Equal(InProgress, checklist.Status());
        }

        [Fact]
        public void NotMultipleChoice()
        {
            var multipleChoiceItem = new MultipleChoiceItem("India","Srilanka");
            var notItem = multipleChoiceItem.Not();
            var checklist = new Checklist( creator, notItem);
            Assert.Equal(InProgress, checklist.Status());
            multipleChoiceItem.Be("India");
            Assert.Equal(Failed, checklist.Status());
            multipleChoiceItem.Be("Srilanka");
            Assert.Equal(Failed, checklist.Status());
            multipleChoiceItem.Be("Bangladesh");
            Assert.Equal(Succeeded, checklist.Status());
            multipleChoiceItem.Reset();
            Assert.Equal(InProgress, checklist.Status());
        }
    }
}
