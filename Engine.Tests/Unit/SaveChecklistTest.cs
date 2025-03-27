using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Items;
using Engine.Persons;
using Xunit;

namespace Engine.Tests.Unit
{
    public class SaveChecklistTest
    {
        private static readonly Person Creator = new Person(0, 0);
        [Fact]
        public void SaveSingleLevelChecklist()
        {

            var checklist = Creator.Checklist(
                "first question".TrueFalse(),
                "second question".TrueFalse(),
                "third question".TrueFalse()
            );
            var item1 = checklist.I(0, 0);
            var item2 = checklist.I(0, 1);
            var item3 = checklist.I(0, 2);
            var memento = checklist.ToMemento();
        }
    }
}
