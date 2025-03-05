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
    public class OrItemTest
    {
        [Fact]
        public void BooleanItems() {
            var item1 = new BooleanItem();
            var item2 = new BooleanItem();
            var item = item1.Or(item2);
            var checkList = new Checklist(item);
            Assert.Equal(InProgress, checkList.Status());
            item1.Be(false);
            Assert.Equal(Failed, checkList.Status());
            item2.Be(true);
            Assert.Equal(Failed, checkList.Status());
            item1.Reset();
            Assert.Equal(Succeeded, checkList.Status());
            item2.Reset();
            Assert.Equal(InProgress, checkList.Status());
        }

    }
}
