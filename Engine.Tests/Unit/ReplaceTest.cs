using System;
using Engine.Items;
using Engine.Persons;
using Xunit;
using Xunit.Abstractions;
using static Engine.Tests.Unit.CarpetColor;
using static Engine.Tests.Unit.MultipleChoiceItemTest;
using static Engine.Items.ChecklistExtensions;
using static Engine.Items.PrettyPrintOptions;

namespace Engine.Tests.Unit {
    public class ReplaceTest {
        private static readonly Person Creator = new Person(0, 0);
        private readonly ITestOutputHelper _testOutput;
        private Checklist checklist;

        // Common setup for MOST tests
        public ReplaceTest(ITestOutputHelper testOutput) {
            _testOutput = testOutput;
            checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess: "Second success leg".TrueFalse(), // successItem2 to be replaced
                        onFailure: "Second failure leg".TrueFalse()
                    ),
                    onFailure: Or(
                        Not(new BooleanItem("First Or of first failure leg")),
                        new BooleanItem("Second Or of first failure leg")
                    )
                ),
                "Last simple item".TrueFalse()
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void ReplaceNotItem() {
            checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
                Not(
                    "Is US citizen?".TrueFalse() // Item2
                )
            );
            Assert.Equal(2, new QuestionCount(checklist).Count);

            var item2 = checklist.P(0, 1, 0);
            Creator.Replace(item2)
                .With(
                    "Vehicle Type?".Choices("Car", "Bike", "Bus"),
                    "Is US citizen?".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(3, new QuestionCount(checklist).Count);
            AssertMissing(item2);
        }

        [Fact]
        public void IllegalReplaceOfItemWithSomethingWithTheItem() {
            checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
                Not(
                    "Is US citizen?".TrueFalse()
                )
            );
            Assert.Equal(2, new QuestionCount(checklist).Count);

            var firstItem = checklist.P(0, 0);
            var notFirstItem = Not(firstItem);
            Assert.Throws<InvalidOperationException>(() => Creator
                .Replace(firstItem)
                .With(notFirstItem)
                .In(checklist));
        }

        [Fact]
        public void ReplaceSuccessLegOfInnerConditional() {
            var successItem2 = checklist.P(0, 1, 1, 1);
            Creator.Replace(successItem2)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Equal(9, new QuestionCount(checklist).Count);
            AssertMissing(successItem2);
        }

        [Fact]
        public void ReplaceConditionOfInnerConditional() {
            var baseItem2 = checklist.P(0, 1, 1, 0);
            Creator.Replace(baseItem2)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(9, new QuestionCount(checklist).Count);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            AssertMissing(baseItem2);
        }

        [Fact]
        public void ReplaceNotContents() {
            var failItem1A = checklist.P(0, 1, 2, 0, 0);
            Creator.Replace(failItem1A)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(9, new QuestionCount(checklist).Count);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            AssertMissing(failItem1A);
        }

        [Fact]
        public void ReplaceInnerConditional() {
            var secondConditional = checklist.P(0, 1, 1);
            Creator.Replace(secondConditional)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(7, new QuestionCount(checklist).Count);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            AssertMissing(secondConditional);
        }

        [Fact]
        public void ReplaceOuterConditionalTwice() {
            var firstConditional = checklist.P(0, 1);
            Creator.Replace(firstConditional)
                .With(
                    "Replace1".TrueFalse(),
                    "Replace2".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(4, new QuestionCount(checklist).Count);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            AssertMissing(firstConditional);

            var newGroup = checklist.P(0, 1);
            Creator.Replace(newGroup) // Replace what we just replaced!
                .With(
                    "Replace3".TrueFalse(),
                    "Replace4".TrueFalse(),
                    "Replace5".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(5, new QuestionCount(checklist).Count);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
        }

        [Fact]
        public void InsertMoreAfterInnerConditionalInSuccessLeg() {
            var innerConditional = checklist.P(0, 1, 1);
            Creator
                .Insert(
                    "Addition1".TrueFalse(),
                    "Addition2".TrueFalse()
                )
                .After(innerConditional)
                .In(checklist);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Equal(10, new QuestionCount(checklist).Count);
            var positions = checklist.Positions(innerConditional);
            Assert.Single(positions);
            Assert.Equal("P[0.1.1.0]", positions[0].ToString()); // Conditional now first in the added Group
        }

        [Fact]
        public void InsertMoreBeforeInnerConditionalInSuccessLeg() {
            var innerConditional = checklist.P(0, 1, 1);
            Creator
                .Insert(
                    "Addition1".TrueFalse(),
                    "Addition2".TrueFalse()
                )
                .Before(innerConditional)
                .In(checklist);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Equal(10, new QuestionCount(checklist).Count);
            var positions = checklist.Positions(innerConditional);
            Assert.Single(positions);
            Assert.Equal("P[0.1.1.2]", positions[0].ToString()); // Conditional now last in the added Group
        }

        [Fact]
        public void ReplaceMultipleInstances() {
            var target = "Item to remove".TrueFalse(); // To use a Question in multiple places, define it once
            checklist = Creator.Checklist(
                target,
                "Second item".TrueFalse(),
                "Third item".TrueFalse(),
                Conditional(
                    condition: "Base condition".TrueFalse(),
                    onSuccess: "Success condition".TrueFalse(),
                    onFailure: target)
            );
            Assert.Equal(2, checklist.Positions(target).Count);
            
            Creator
                .Replace(target)
                .With("Replacement".TrueFalse())
                .In(checklist);
            _testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Throws<ArgumentException>(() => new CurrentAnswers(checklist).Value("Item to remove")); // It's gone!
        }

        [Fact]
        public void ReplaceItemInGroup() {
            checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
                "Is US citizen?".TrueFalse(), // Item2
                "Which country?".Choices("India", "Iceland", "Norway")
            );
            Assert.Equal(3, new QuestionCount(checklist).Count);

            var item2 = checklist.P(0, 1);
            Creator.Replace(item2)
                .With(
                    "Vehicle Type?".Choices("Car", "Bike", "Bus"),
                    "Is US citizen?".TrueFalse()
                )
                .In(checklist);
            Assert.Equal(4, new QuestionCount(checklist).Count);
            AssertMissing(item2);
        }

        private void AssertMissing(Item item) {
            var positions = checklist.Positions(item);
            Assert.True(positions.Count == 0, 
                $"Unexpectedly located item <{item}> in Checklist hierarchy at {string.Join(", ", positions)}");
        }
    }
}