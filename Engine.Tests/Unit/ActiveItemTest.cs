using Engine.Items;
using Engine.Persons;
using Xunit;
using Xunit.Abstractions;
using static Engine.Tests.Unit.MultipleChoiceItemTest;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit {
    public class ActiveItemTest {
        private static readonly Person Creator = new Person(0, 0);
        private readonly ITestOutputHelper _testOutput;
        private Checklist checklist;

        // Common setup for MOST tests
        public ActiveItemTest(ITestOutputHelper testOutput) {
            _testOutput = testOutput;
            checklist = Creator.Checklist(
                "First simple item".TrueFalse(), // 0.0
                Conditional( // 0.1
                    condition: "First condition".TrueFalse(), // 0.1.0
                    onSuccess: Conditional( // 0.1.1
                        condition: "Second condition".TrueFalse(), // 0.1.1.0
                        onSuccess: "Second success leg".TrueFalse(), // 0.1.1.1
                        onFailure: "Second failure leg".TrueFalse() // 0.1.1.2
                    ),
                    onFailure: Or( // 0.1.2
                        Not(new BooleanItem("First Or of first failure leg")), // 0.1.2.0  0.1.1.2.0.0
                        new BooleanItem("Second Or of first failure leg") // 0.1.2.1
                    )
                ),
                "Last simple item".TrueFalse() // 0.2
            );
            Assert.Equal(8, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void ActiveItemsWithoutSetting() {
            var activeItems = checklist.ActiveItems();
            Assert.Equal(3, activeItems.Count);
            
            var topCondition = (SimpleItem)checklist.I(0, 1, 0);
            Creator.Sets(topCondition).To(false);
            activeItems = checklist.ActiveItems();
            Assert.Equal(5, activeItems.Count);
            
            Creator.Sets(topCondition).To(true);
            activeItems = checklist.ActiveItems();
            Assert.Equal(4, activeItems.Count);
            
            var innerCondition = (SimpleItem)checklist.I(0, 1, 1, 0);
            Creator.Sets(innerCondition).To(true);
            activeItems = checklist.ActiveItems();
            Assert.Equal(5, activeItems.Count);
        }
    }
}