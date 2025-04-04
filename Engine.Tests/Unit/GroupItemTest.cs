using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Engine.Items;
using Engine.Persons;
using Xunit;

namespace Engine.Tests.Unit
{   
   
    public class GroupItemTest
    {
        private static readonly Person Creator = new Person(0, 0);
        [Fact]
        public void Group()
        {
            var checklist = Creator.Checklist( 
                    "first question".TrueFalse(),
                    "second question".TrueFalse(),
                    "third question".TrueFalse()
                );
            var item1 = (SimpleItem)checklist.I(0, 0);
            var item2 = (SimpleItem)checklist.I(0, 1);
            var item3 = (SimpleItem)checklist.I(0, 2);
            
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item1).To(true);
            Creator.Sets(item2).To(true);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item3).To(true);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());

            Creator.Sets(item2).To(false);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status()); 
            Creator.Reset(item2);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());

        }
    }
}
