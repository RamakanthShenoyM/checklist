using Engine.Items;
using Engine.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
    public class OrItemTest
    {
		private readonly static Person creator = new Person();
		[Fact]
        public void BooleanItems() {
            var item1 = new BooleanItem();
            var item2 = new BooleanItem();
            var item = item1.Or(item2);
            var checkList = new Checklist(creator, item);
            Assert.Equal(InProgress, checkList.Status());
            creator.Sets(item1).To(false);
            Assert.Equal(Failed, checkList.Status());
            creator.Sets(item2).To(true);
            Assert.Equal(Failed, checkList.Status());
            creator.Reset(item1);
            Assert.Equal(Succeeded, checkList.Status());
            creator.Reset(item2);
            Assert.Equal(InProgress, checkList.Status());
        }

    }
}
