using Engine.Items;
using Engine.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Engine.Items.ChecklistStatus;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit
{
    public class OrItemTest
    {
		private static readonly Person Creator = new Person(0, 0);
		[Fact]
        public void BooleanItems() {
            var checklist = Creator.Checklist(
                Or(
                    new BooleanItem("Is US citizen?"),
                    new BooleanItem("Is US citizen?")
                    )
            );
            var item1 = checklist.I(0, 0);
            var item2 = checklist.I(0, 1);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(item1).To(false);
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(item2).To(true);
            Assert.Equal(Failed, checklist.Status());
            Creator.Reset(item1);
            Assert.Equal(Succeeded, checklist.Status());
            Creator.Reset(item2);
            Assert.Equal(InProgress, checklist.Status());
        }

    }
}
