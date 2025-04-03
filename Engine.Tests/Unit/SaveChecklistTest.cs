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
            var restoredChecklist = Checklist.FromMemento(memento);
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

        [Fact]
        public void SaveComplexChecklist()
        {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess: "Second success leg".Choices("A","B","C"),
                        onFailure: "Second failure leg".TrueFalse()
                    ),
                    onFailure: Not(
                        Or(
                            "First Or of first failure leg".TrueFalse(),
                            "Second Or of first failure leg".TrueFalse()
                        )
                    )
                ),
                "Last simple item".TrueFalse()
            );
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);
        }

		[Fact]
		public void SimpleChecklistWithMultichoice()
		{
            var item1 = "First Item".Choices("A", "B", "C");
			var checklist = Creator.Checklist(
				item1
			);
			Creator.Sets(item1).To("A");
			var memento = checklist.ToMemento();
			var restoredChecklist = Checklist.FromMemento(memento);
			testOutput.WriteLine(memento);
		}
        
        [Fact]
		public void CheckEqualsForBooleanItemChecklist()
        {
            var item1 = "First Item".TrueFalse();
			var checklist = Creator.Checklist(
				item1
			);
			Creator.Sets(item1).To(true);
			var memento = checklist.ToMemento();
			var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);
		}

        [Fact]
        public void CheckEqualsForGroupItemCheckList()
        {
            var item1 = Group(
                "first inner question".TrueFalse(),
                "second inner question".TrueFalse()
            );
            var checklist = Creator.Checklist(
                item1
            );
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);
        }

        [Fact]
        public void CheckEqualsForOrItemCheckList()
        {
            var item1 = Or(
                "first inner question".TrueFalse(),
                "second inner question".TrueFalse()
            );
            var checklist = Creator.Checklist(
                item1
            );
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);
        }

        [Fact]
        public void CheckEqualsForNotItemCheckList()
        {
            var item1 = Not(
                Group(
                    "first inner question".TrueFalse(),
                    "second inner question".TrueFalse()
                )
            );
            var checklist = Creator.Checklist(
                item1
            );
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);
        }

        [Fact]
        public void CheckEqualsForConditionalItemCheckList()
        {
            var item1 = Conditional(
                "Conditional question".TrueFalse(),
                Group(
                    "first Succeed inner question".TrueFalse(),
                    "second Succeed inner question".TrueFalse()
                ),
                "Failed question".TrueFalse()
            );
            var checklist = Creator.Checklist(
                item1
            );
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);
        }

        [Fact]
        public void CheckEqualsForMultipleChoiceItem()
        {
            var item1 = "second question".Choices(1, 2, 3,4);
            var checklist = Creator.Checklist(
                item1
            );
            Creator.Sets(item1).To(1);
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);

        }

        [Fact]
        public void CheckEqualsForComplexCheckList()
        {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess: "Second success leg".TrueFalse(), 
                        onFailure: "Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not(new BooleanItem("First Or of first failure leg")),
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );

            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
        }

        [Fact]
        public void MissingFailureLegInConditionalInCheckList()
        {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess: "Second success leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not(new BooleanItem("First Or of first failure leg")),
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            var memento= checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
        }

        [Fact]
        public void MissingSuccessLegInConditionalInCheckList()
        {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onFailure: "Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not(new BooleanItem("First Or of first failure leg")),
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            var memento= checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
        }
    }
}
