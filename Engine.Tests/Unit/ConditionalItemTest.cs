using Engine.Items;
using Engine.Persons;
using System;
using Xunit;
using Xunit.Abstractions;
using static Engine.Items.ChecklistStatus;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit {
    public class ConditionalItemTest(ITestOutputHelper testOutput) {
        private static readonly Person Creator = new(0, 0);

        [Fact]
        public void Boolean() {
            var checklist = Creator.Checklist(
                Conditional(
                    condition: "Is US citizen?".TrueFalse(),
                    onSuccess: "Is Nevada resident?".TrueFalse(),
                    onFailure: "Is Canadian citizen?".TrueFalse()
                ));
            var baseItem = checklist.I(0, 0);
            var successItem = checklist.I(0, 1);
            var failItem = checklist.I(0, 2);

            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(baseItem).To(true);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(successItem).To(false);
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(baseItem).To(false);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(failItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
            var output = new PrettyPrint(checklist).Result();
            testOutput.WriteLine(output);
        }

        [Fact]
        public void BooleanWithFalse() {
            var checklist = Creator.Checklist(
                Conditional(
                    condition: "Is US citizen?".TrueFalse(),
                    onFailure: "Is Canadian citizen?".TrueFalse()
                ));
            var baseItem = checklist.I(0, 0);
            var failItem = checklist.I(0, 2);

            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(baseItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
            Creator.Sets(baseItem).To(false);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(failItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
        }

        [Fact]
        public void BooleanWithTrue() {
            var checklist = Creator.Checklist(
                Conditional(
                    condition: "Is US citizen?".TrueFalse(),
                    onSuccess: "Is Nevada resident?".TrueFalse()
                ));
            var baseItem = checklist.I(0, 0);
            var successItem = checklist.I(0, 1);

            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(baseItem).To(false);
            Assert.Equal(Failed, checklist.Status());
            Creator.Sets(baseItem).To(true);
            Assert.Equal(InProgress, checklist.Status());
            Creator.Sets(successItem).To(true);
            Assert.Equal(Succeeded, checklist.Status());
        }

        [Fact]
        public void ConditionalWithConditional() {
            var checklist = Creator.Checklist(
                "First simple item".TrueFalse(),
                Conditional(
                    condition: "First condition".TrueFalse(),
                    onSuccess: Conditional(
                        condition: "Second condition".TrueFalse(),
                        onSuccess: "Second success leg".TrueFalse(),
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
            testOutput.WriteLine(checklist.ToString());
        }
    }
}