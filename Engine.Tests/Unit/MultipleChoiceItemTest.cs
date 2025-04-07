using Engine.Items;
using System;
using System.Collections.Generic;
using static Engine.Tests.Unit.CarpetColor;
using Xunit;
using Engine.Persons;
using Xunit.Abstractions;
using Engine.Tests.Util;
using static Engine.Items.ChecklistEventType;
using CommonUtilities.Util;

namespace Engine.Tests.Unit
{
    public class MultipleChoiceItemTest(ITestOutputHelper testOutput)
    {
        private static readonly Person Creator = new Person(0, 0);

        [Fact]
        public void SingleItem()
        {
            var checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet)
            );
            var item = (SimpleItem)checklist.I(0);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item).To(GreenCarpet);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
            Creator.Sets(item).To(BlueCarpet);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());
            Creator.Reset(item);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Assert.Throws<InvalidOperationException>(() => Creator.Sets(item).To("India"));
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => Creator.Sets(item).To(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            var history = new HistoryDump(checklist).Result;
            testOutput.WriteLine(history.ToString());
            Assert.Equal(2, history.Events(SetValueEvent).Count);
            Assert.Single(history.Events(ResetValueEvent));
            Assert.Equal(2, history.Events(InvalidValueEvent).Count);
        }

        [Fact]
        public void DontAllowMixedType()
        {
            Assert.Throws<ArgumentException>(() => Creator.Checklist(
                "Which Carpet Color?".Choices("Red", GreenCarpet, NoCarpet)
            ));

            Assert.Throws<ArgumentException>(() => Creator.Checklist(
                "Which Carpet Color?".Choices(1, 2, NoCarpet)
            ));

            Assert.Throws<ArgumentException>(() => Creator.Checklist(
                "Which Carpet Color?".Choices(GreenCarpet, NoCarpet, 'C')
            ));

        }

        [Fact]
        public void MixedItems()
        {
            var checklist = Creator.Checklist(
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
                ("Is US citizen?").TrueFalse(),
                "Which country?".Choices("India", "Iceland", "Norway")
            );
            var item1 = (SimpleItem)checklist.I(0, 0);
            var item2 = (SimpleItem)checklist.I(0, 1);
            var item3 = (SimpleItem)checklist.I(0, 2);

            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            Creator.Sets(item1).To(GreenCarpet);
            Creator.Sets(item2).To(true);
            Creator.Sets(item3).To("India");
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
            Creator.Sets(item3).To("Poland");
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());

            var answers = new CurrentAnswers(checklist);
            Assert.Equal(GreenCarpet, answers.Value("Which Carpet Color?"));
            Assert.Equal("Poland", answers.Value("Which country?"));
            Assert.Equal(true, answers.Value("Is US citizen?"));
        }

        internal class QuestionCount : ChecklistVisitor
        {
            internal int Count;

            public QuestionCount(Checklist checklist)
            {
                checklist.Accept(this);
            }

            public void Visit(BooleanItem item, Guid id, string question, bool? value, Dictionary<Person, List<Operation>> operations,History history) =>
                Count++;

            public void Visit(MultipleChoiceItem item, Guid id, string question, object? value, List<object> choices,
                Dictionary<Person, List<Operation>> operations, History history) =>
                Count++;
        }

    }
    internal enum CarpetColor
    {
        RedCarpet,
        GreenCarpet,
        NoCarpet,
        BlueCarpet
    }
}
