using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Engine.Items;
using Engine.Persons;
using Xunit;
using Xunit.Abstractions;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit
{
    public class SaveChecklistTest(ITestOutputHelper testOutput)
    {
        private static readonly Person Creator = new Person(0, 0);
        [Fact]
        public void SaveGroupChecklist()
        {
            var checklist = Creator.Checklist(
                "first question".TrueFalse(),
                Group(
                    "first inner question".TrueFalse(),
                    "second inner question".TrueFalse()
                    ),
                "second question".TrueFalse(),
                "third question".TrueFalse()
            );
            var memento = checklist.ToMemento();
            testOutput.WriteLine(memento);
        }
        [Fact]
        public void SaveOrChecklist()
        {
            var checklist = Creator.Checklist(
                "first question".TrueFalse(),
                Or(
                    "first inner question".TrueFalse(),
                    "second inner question".TrueFalse()
                    ),
                "second question".TrueFalse(),
                "third question".TrueFalse()
            );
            var memento = checklist.ToMemento();
            testOutput.WriteLine(memento);
        }
        [Fact]
        public void SaveNotChecklist()
        {
            var checklist = Creator.Checklist(
                "first question".TrueFalse(),
                Not(
                    Group(
                        "first inner question".TrueFalse(),
                        "second inner question".TrueFalse()
                    )
                    ),
                "second question".TrueFalse(),
                "third question".TrueFalse()
            );
            var memento = checklist.ToMemento();
            testOutput.WriteLine(memento);
        }
        [Fact]
        public void SaveConditionalChecklist()
        {
            var checklist = Creator.Checklist(
                "first question".TrueFalse(),
            Conditional(
                    "Conditional question".TrueFalse(),
                    Group(
                        "first Succeed inner question".TrueFalse(),
                        "second Succeed inner question".TrueFalse()
                    ),
                    "Failed question".TrueFalse()
                    ),
                "second question".Choices("A", "B", "C", "D"),
                "third question".TrueFalse()
            );
            var memento = checklist.ToMemento();
            testOutput.WriteLine(memento);
        }
    }
}
