﻿using Engine.Items;
using Engine.Persons;
using Engine.Tests.Util;
using Xunit;
using Xunit.Abstractions;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit
{
    public class ChecklistTest(ITestOutputHelper testOutput)
    {
        private static readonly Person Creator = new Person(0, 0);
        private static readonly Person Owner = new Person(1, 2);
        [Fact]
        public void CloneChecklistForBooleanItem()
        {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
        }
        [Fact]
        public void CloneChecklistForMultipleChoiceItem()
        {
            var item1 = "second question".Choices(1, 2, 3, 4);
            var checklist = Creator.Checklist(
                item1
            );
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
        }
        [Fact]
        public void CloneChecklistForGroupItem()
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
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
        }
        [Fact]
        public void CloneChecklistForOrItem()
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
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
        }
        [Fact]
        public void CloneChecklistForNotItem()
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
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
        }
        [Fact]
        public void CloneChecklistForConditionalItem()
        {
            var checklist = Creator.Checklist(
                "first question".TrueFalse(),
                Conditional(
                    "Conditional question".TrueFalse(),
                    Group(
                        "first Succeed inner question".TrueFalse(),
                        "second Succeed inner question".TrueFalse()
                    )),
                    "Failed question".TrueFalse(),
                    Not(
                        Group(
                            "first inner question".TrueFalse(),
                            "second inner question".TrueFalse()
                        )
                    ), Or(
                        "first inner question".TrueFalse(),
                        "second inner question".TrueFalse()
                    ),
                "second question".Choices("A", "B", "C", "D"),
                "third question".TrueFalse()
            );
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
            var item1 = (SimpleItem)checklistClone.P(0, 0);
            Creator.Sets(item1).To(true);
            Assert.NotEqual(checklist, checklistClone);
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
        }
        [Fact]
        public void CloneChecklistForComplexMultipleChoiceItem()
        {
            var checklist = Creator.Checklist(
                "first question".Choices("Option 1", "Option 2", "Option 3"),
                Conditional(
                    "Conditional question".Choices("Yes", "No"),
                    Group(
                        "first Succeed inner question".Choices("A", "B", "C"),
                        "second Succeed inner question".Choices(1, 2, 3, 4)
                    )),
                "Failed question".Choices("X", "Y", "Z"),
                Not(
                    Group(
                        "first inner question".Choices("True", "False"),
                        "second inner question".Choices("On", "Off")
                    )
                ), Or(
                    "first inner question".Choices("Red", "Blue", "Green"),
                    "second inner question".Choices("Hot", "Cold")
                ),
                "second question".Choices("A", "B", "C", "D"),
                "third question".Choices("Yes", "No")
            );
            
            var checklistClone = checklist.Clone();
            Assert.Equal(checklist, checklistClone);
            var item2 = (SimpleItem)checklistClone.P(0, 1, 1, 0);
            Creator.Sets((SimpleItem)checklistClone.P(0, 0)).To("Option 1");
            Creator.Add(Owner).As(Role.Owner).To(item2);
            Owner.Sets(item2).To("A");
            Assert.NotEqual(checklist, checklistClone);
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
        }

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
		public void BooleanItemChecklistEquality()
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
        public void BooleanItemChecklistEqualityWithMultiplePersons()
        {
            var item1 = "First Item".TrueFalse();
            var item2 = "Second Item".TrueFalse();
            var checklist = Creator.Checklist(
                item1,item2
            );
            Creator.Sets(item1).To(true);
            Creator.Add(Owner).As(Role.Owner).To(item2);
            Owner.Sets(item2).To(false);
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
        public void CheckEqualsForGroupItemCheckListWithMultiplePersons()
        {
            var item1 = Group(
                "first inner question".TrueFalse(),
                "second inner question".TrueFalse()
            );
            var checklist = Creator.Checklist(
                item1
            );
            Creator.Add(Owner).As(Role.Owner).To(item1);
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
        public void CheckEqualsForOrItemCheckListWithMultiplePerson()
        {
            var item1 = Or(
                "first inner question".TrueFalse(),
                "second inner question".TrueFalse()
            );
            var checklist = Creator.Checklist(
                item1
            );
            Creator.Add(Owner).As(Role.Owner).To(item1);
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
        public void CheckEqualsForNotItemCheckListWithMultiplePersons()
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
            Creator.Add(Owner).As(Role.Owner).To(item1);
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
        public void CheckEqualsForConditionalItemCheckListWithMultiplePersons()
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
            Creator.Add(Owner).As(Role.Owner).To(item1);
            var memento = checklist.ToMemento();
            var restoredChecklist = Checklist.FromMemento(memento);
            Assert.Equal(checklist, restoredChecklist);
            testOutput.WriteLine(memento);
        }

        [Fact]
        public void MultipleChoiceItemChecklistEquality()
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
        public void MultipleChoiceItemChecklistEqualityWithMultiplePersons()
        {
            var item1 = "second question".Choices(1, 2, 3, 4);
            var item2 = "second question".Choices("A", "B", "C", "D");
            var checklist = Creator.Checklist(
                item1,item2
            );
            Creator.Sets(item1).To(1);
            Creator.Add(Owner).As(Role.Owner).To(item2);
            Owner.Sets(item2).To("A");
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
