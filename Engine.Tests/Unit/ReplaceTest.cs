using System;
using Engine.Items;
using Engine.Persons;
using Xunit;
using Xunit.Abstractions;
using static Engine.Tests.Unit.CarpetColor;
using static Engine.Tests.Unit.MultipleChoiceItemTest;
using static Engine.Items.ChecklistExtensions;
using static Engine.Items.PrettyPrint.PrettyPrintOptions;

namespace Engine.Tests.Unit {
    public class ReplaceTest(ITestOutputHelper testOutput) {
        private static readonly Person Creator = new Person(0, 0);

        [Fact]
        public void ReplaceItem() {
            var checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
                "Is US citizen?".TrueFalse(),     // Item2
                "Which country?".Choices("India", "Iceland", "Norway")
            );
            Assert.Equal(3, new QuestionCount(checklist).Count);

            var item2 = checklist.I(0, 1);
            Creator.Replace(item2)
                .With(
                    "Vehicle Type?".Choices("Car", "Bike", "Bus"),
                    "Is US citizen?".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(4, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void NotItem() {
            var checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
                Not(
                    "Is US citizen?".TrueFalse() // Item2
                    )
            );
            Assert.Equal(2, new QuestionCount(checklist).Count);

            var item2 = checklist.I(0,1,0) ;
            Creator.Replace(item2)
                .With(
                    "Vehicle Type?".Choices("Car", "Bike", "Bus"),
                    "Is US citizen?".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(3, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void IllegalReplace() {
            var checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
                Not(
                    "Is US citizen?".TrueFalse() // Item2
                )
            );
            Assert.Equal(2, new QuestionCount(checklist).Count);
            
            var item1 = checklist.I(0,0);
            Assert.Throws<InvalidOperationException>(() => 
                Creator.Replace(item1).With(item1.Not()).In(checklist));
        }

        [Fact]
        public void ConditionalWithConditional() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess:"Second success leg".TrueFalse(),  // successItem2 to be replaced
                        onFailure:"Second failure leg".TrueFalse()
                        ),
                    onFailure: Or(
                        Not( new BooleanItem("First Or of first failure leg") ),
                        new BooleanItem("Second Or of first failure leg")
                        )
                    ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);

            var successItem2 = checklist.I(0,1,1,1);
            Creator.Replace(successItem2)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                    )
                .In(checklist);
            testOutput.WriteLine(checklist.ToString());
            Assert.Equal(9, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void ReplacingConditionalBase() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),  // replace this condition
                        onSuccess:"Second success leg".TrueFalse(),  
                        onFailure:"Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not( new BooleanItem("First Or of first failure leg") ),
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);
            
            var baseItem2 = checklist.I(0,1,1,0);
            Creator.Replace(baseItem2)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(9, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString(NoOperations));
        }

        [Fact]
        public void ReplacingOrLeg() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess:"Second success leg".TrueFalse(),  
                        onFailure:"Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not( new BooleanItem("First Or of first failure leg") ), // inner question to be replaced
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);
            
            var failItem1A = checklist.I(0,1,2,0,0);
            Creator.Replace(failItem1A)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(9, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString(NoOperations));
        }

        [Fact]
        public void ReplacingInnerConditional() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(                  // Replace entire Conditional
                        condition: "Second condition".TrueFalse(),
                        onSuccess:"Second success leg".TrueFalse(),  
                        onFailure:"Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not( new BooleanItem("First Or of first failure leg") ), // inner question to be replaced
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);
            
            var secondConditional = checklist.I(0,1,1);
            Creator.Replace(secondConditional)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(7, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString(NoOperations));
        }

        [Fact]
        public void ReplacingOuterConditional() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(                  // Replace entire Conditional
                        condition: "Second condition".TrueFalse(),
                        onSuccess:"Second success leg".TrueFalse(),  
                        onFailure:"Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not( new BooleanItem("First Or of first failure leg") ), // inner question to be replaced
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);
            
            var firstConditional = checklist.I(0,1);
            Creator.Replace(firstConditional)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(4, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString(NoOperations));
            
            var newGroup = checklist.I(0,1);
            Creator.Replace(newGroup)  // Replace what we just replaced!
                .With(
                    "Replace3".TrueFalse(),
                    "Replace4".TrueFalse(),
                    "Replace5".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(5, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString(NoOperations));
        }

        [Fact]
        public void InsertAfterToConditional() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(                  // Insert two more here
                        condition: "Second condition".TrueFalse(),
                        onSuccess:"Second success leg".TrueFalse(),  
                        onFailure:"Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not( new BooleanItem("First Or of first failure leg") ), 
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);

            var innerConditional = checklist.I(0, 1, 1);
            Creator
                .Insert(
                    "Addition1".TrueFalse(),
                    "Addition2".TrueFalse()
                    )
                .After(innerConditional)
                .In(checklist);
            testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Equal(10, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void InsertBeforeToConditional() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(                  
                        condition: "Second condition".TrueFalse(),
                        onSuccess:"Second success leg".TrueFalse(),  
                        onFailure:"Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not( new BooleanItem("First Or of first failure leg") ), 
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);
            // Insert two more on success leg of inner conditional
            var innerConditional = checklist.I(0, 1, 1);
            Creator
                .Insert(
                    "Addition1".TrueFalse(),
                    "Addition2".TrueFalse()
                )
                .Before(innerConditional)
                .In(checklist);
            testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Equal(10, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void ReplaceMultipleInstances() {
            var target = "Item to remove".TrueFalse(); // To use a Question in multiple places, define it once
            var checklist = Creator.Checklist(
                target,
                "Second item".TrueFalse(),
                "Third item".TrueFalse(),
                Conditional(
                    condition: "Base condition".TrueFalse(),
                    onSuccess: "Success condition".TrueFalse(),
                    onFailure: target)
            );
            Creator
                .Replace(target)
                .With("Replacement".TrueFalse())
                .In(checklist);
            testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Throws<ArgumentException>(() => new CurrentAnswers(checklist).Value("Item to remove")); // It's gone!
        }
    }
}