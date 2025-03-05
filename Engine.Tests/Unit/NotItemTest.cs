using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
    public class NotItemTest
    {

        [Fact]
        public void NotBoolean()
        {
            var booleanItem = new BooleanItem();
            var notItem = booleanItem.Not();
            var checklist = new Checklist(notItem);
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
            var checklist = new Checklist(notItem);
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
