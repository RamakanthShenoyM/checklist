using System;
using Engine.Items;
using Engine.Persons;
using Engine.Tests.Util;
using Xunit;
using Xunit.Abstractions;
using static Engine.Items.ChecklistEventType;

namespace Engine.Tests.Unit {
    public class BooleanItemTest(ITestOutputHelper testOutput) {
        private static readonly Person Creator = new Person(0, 0);

        [Fact]
        public void SingleItem() {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var item = (SimpleItem)checklist.P(0);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item).To(true);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
            Creator.Sets(item).To(false);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());
            Creator.Reset(item);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item).To(true);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
                #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => Creator.Sets(item).To(null));
                #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<InvalidCastException>(() => Creator.Sets(item).To("India"));
        }
        
        [Fact]
        public void SingleItemHistory() {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var item = (SimpleItem)checklist.P(0);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item).To(true);
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
            Assert.Single(history.Events(SetValueEvent));
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
        }

        [Fact]
        public void MultipleItems() {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse(),
                "Is Indian citizen?".TrueFalse(),
                "Is Nordic citizen?".TrueFalse()
            );
            var item1 = (SimpleItem)checklist.P(0, 0);
            var item2 = (SimpleItem)checklist.P(0, 1);
            var item3 = (SimpleItem)checklist.P(0, 2);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item1).To(true);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());

            Creator.Sets(item2).To(true);
            Creator.Sets(item3).To(true);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
            Creator.Sets(item2).To(false);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());

            Creator.Reset(item1);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());
            Creator.Reset(item2);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            var answers = new CurrentAnswers(checklist);
            Assert.Null(answers["Is US citizen?"]);
            Assert.Null(answers["Is Indian citizen?"]);
            Assert.Equal(true, answers["Is Nordic citizen?"]);
        }

        [Fact]
        public void InvalidValue() {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var item = (SimpleItem)checklist.P(0);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item).To(true);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
            Assert.Throws<InvalidCastException>(() => Creator.Sets(item).To("green"));
        }
    }
}