using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Engine.Tests.Unit.CarpetColor;
using Xunit;

namespace Engine.Tests.Unit
{
    public class MultipleChoiceItemTest
    {
        [Fact]
        public void SingleItem()
        {
            var item = new MultipleChoiceItem(RedCarpet, GreenCarpet, NoCarpet);
            var checklist = new Checklist(item);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            item.Be(GreenCarpet);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
            item.Be(BlueCarpet);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());
            item.Reset();
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
        }
        [Fact]
        public void EmptyChecklist()
        {
            var item = new MultipleChoiceItem(RedCarpet);
            var checklist = new Checklist(item);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            checklist.Cancel(item);
            Assert.Equal(ChecklistStatus.NotApplicable, checklist.Status());
        }


    }
    internal enum CarpetColor
    {
        RedCarpet, 
        GreenCarpet, 
        NoCarpet,
        BlueCarpet
    }
}
