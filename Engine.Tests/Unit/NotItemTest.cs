using Engine.Items;
using Engine.Persons;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
    public class NotItemTest
    {

        private readonly static Person creator = new Person();
		[Fact]
        public void NotBoolean()
        {
            var booleanItem = new BooleanItem("Is US citizen?");
            var notItem = booleanItem.Not();
            var checklist = new Checklist( creator, notItem);
            Assert.Equal(InProgress, checklist.Status());
            creator.Sets(booleanItem).To(true);
            Assert.Equal(Failed, checklist.Status());
            creator.Sets(booleanItem).To(false);
            Assert.Equal(Succeeded, checklist.Status());
            creator.Reset(booleanItem);
            Assert.Equal(InProgress, checklist.Status());
        }

        [Fact]
        public void NotMultipleChoice()
        {
            var multipleChoiceItem = new MultipleChoiceItem("India","Srilanka");
            var notItem = multipleChoiceItem.Not();
            var checklist = new Checklist( creator, notItem);
            Assert.Equal(InProgress, checklist.Status());
            creator.Sets(multipleChoiceItem).To("India");
            Assert.Equal(Failed, checklist.Status());
            creator.Sets(multipleChoiceItem).To("Srilanka");
            Assert.Equal(Failed, checklist.Status());
            creator.Sets(multipleChoiceItem).To("Bangladesh");
            Assert.Equal(Succeeded, checklist.Status());
            creator.Reset(multipleChoiceItem);
            Assert.Equal(InProgress, checklist.Status());
        }
    }
}
