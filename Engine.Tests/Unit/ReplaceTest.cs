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
        public void ReplacingCompleteConditional() {
            var firstItem = "First simple item".TrueFalse();
            var baseItem1 = new BooleanItem("First condition");
            var baseItem2 = new BooleanItem("Second condition");
            var successItem2 = new BooleanItem("Second success leg");
            var failItem2 = new BooleanItem("Second failure leg");
            var successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
            var failItem1A = new BooleanItem("First Or of first failure leg");
            var failItem1B = new BooleanItem("Second Or of first failure leg");
            var failItem1 = failItem1A.Not().Or(failItem1B);
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new QuestionCount(checklist).Count);
            var replace1 = "Replace1".TrueFalse();
            var replace2 = "Replace2".TrueFalse();
            Creator.Replace(successItem1).With(replace1, replace2).In(checklist);
            Assert.Equal(7, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
        }

        [Fact]
        public void ReplacingTopConditional() {
            var firstItem = "First simple item".TrueFalse();
            var baseItem1 = new BooleanItem("First condition");
            var baseItem2 = new BooleanItem("Second condition");
            var successItem2 = new BooleanItem("Second success leg");
            var failItem2 = new BooleanItem("Second failure leg");
            var successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
            var failItem1A = new BooleanItem("First Or of first failure leg");
            var failItem1B = new BooleanItem("Second Or of first failure leg");
            var failItem1 = failItem1A.Not().Or(failItem1B);
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new QuestionCount(checklist).Count);
            var replace1 = "Replace1".TrueFalse();
            var replace2 = "Replace2".TrueFalse();
            var group1 = new GroupItem(replace1, replace2);
            Creator.Replace(compositeItem).With(group1).In(checklist);
            Assert.Equal(4, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
            var replace3 = "Replace3".TrueFalse();
            var replace4 = "Replace4".TrueFalse();
            var replace5 = "Replace5".TrueFalse();
            Creator.Replace(group1).With(replace3, replace4, replace5).In(checklist);
            Assert.Equal(5, new QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
        }

        [Fact]
        public void InsertAfterToConditional() {
            var firstItem = "First simple item".TrueFalse();
            var baseItem1 = new BooleanItem("First condition");
            var baseItem2 = new BooleanItem("Second condition");
            var successItem2 = new BooleanItem("Second success leg");
            var failItem2 = new BooleanItem("Second failure leg");
            var successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
            var failItem1A = new BooleanItem("First Or of first failure leg");
            var failItem1B = new BooleanItem("Second Or of first failure leg");
            var failItem1 = failItem1A.Not().Or(failItem1B);
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new QuestionCount(checklist).Count);
            var addition1 = "Addition1".TrueFalse();
            var addition2 = "Addition2".TrueFalse();
            var group1 = new GroupItem(addition1, addition2);
            Creator.Insert(addition1, addition2).After(successItem1).In(checklist);
            testOutput.WriteLine(checklist.ToString());
            Assert.Equal(10, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void InsertBeforeToConditional() {
            var firstItem = "First simple item".TrueFalse();
            var baseItem1 = new BooleanItem("First condition");
            var baseItem2 = new BooleanItem("Second condition");
            var successItem2 = new BooleanItem("Second success leg");
            var failItem2 = new BooleanItem("Second failure leg");
            var successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
            var failItem1A = new BooleanItem("First Or of first failure leg");
            var failItem1B = new BooleanItem("Second Or of first failure leg");
            var failItem1 = failItem1A.Not().Or(failItem1B);
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new QuestionCount(checklist).Count);
            var addition1 = "Addition1".TrueFalse();
            var addition2 = "Addition2".TrueFalse();
            Creator.Insert(addition1, addition2).Before(successItem1).In(checklist);
            testOutput.WriteLine(checklist.ToString());
            Assert.Equal(10, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void ReplaceMultipleInstances() {
            var target = "Item to remove".TrueFalse();
            var item2 = "Second item".TrueFalse();
            var item3 = "Third item".TrueFalse();
            var baseItem = "Base condition".TrueFalse();
            var successItem = "Success condition".TrueFalse();
            var conditional = new ConditionalItem(baseItem, successItem, target);
            var checklist = new Checklist(Creator, target, item2, item3, conditional);
            var replacement = "Replacement".TrueFalse();
            Creator.Replace(target).With(replacement).In(checklist);
            testOutput.WriteLine(checklist.ToString(NoOperations));
            Assert.Throws<ArgumentException>(() => new CurrentAnswers(checklist).Value("Item to remove"));
        }
    }
}