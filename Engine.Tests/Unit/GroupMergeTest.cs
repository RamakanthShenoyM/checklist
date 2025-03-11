using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Items;
using Engine.Persons;
using Xunit;
using Xunit.Abstractions;
using static Engine.Tests.Unit.MultipleChoiceItemTest;
using static Engine.Tests.Unit.GroupMergeTest.QuesitonType;

namespace Engine.Tests.Unit;

// Ensures that GroupItems can be merged
public class GroupMergeTest(ITestOutputHelper testOutput) {
    private static readonly Person Creator = new();

    [Fact]
    public void SingleGroupReplace() {
        var item1 = "Item1".TrueFalse();
        var item2 = "Item2".TrueFalse();
        var item3 = "Item3".TrueFalse();
        var group = new GroupItem(item1, item2, item3);
        var checklist = new Checklist(Creator, group);
        Assert.Equal(3, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(2, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(false));

        var replace2A = "Replace2A".TrueFalse();
        var replace2B = "Replace2B".TrueFalse();
        var replace2C = "Replace2C".TrueFalse();
        Creator.Replace(item2).With(replace2A, replace2B, replace2C).In(checklist);
        Assert.Equal(5, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(3, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(false));

        checklist.Simplify();
        Assert.Equal(5, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(2, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(false));
    }

    [Fact]
    public void MultipleGroupReplace() {
        var item1 = "Item1".TrueFalse();
        var item2 = "Item2".TrueFalse();
        var item3 = "Item3".TrueFalse();
        var item4 = "Item4".TrueFalse();
        var group = new GroupItem(item1, item2, item3, item4);
        var checklist = new Checklist(Creator, group);
        Assert.Equal(4, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(2, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(false));

        var replace2A = "Replace2A".TrueFalse();
        var replace2B = "Replace2B".TrueFalse();
        var replace2C = "Replace2C".TrueFalse();
        Creator.Replace(item2).With(replace2A, replace2B, replace2C).In(checklist);
        Assert.Equal(6, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(3, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(false));

        var replace4A = "Replace4A".TrueFalse();
        var replace4B = "Replace4B".TrueFalse();
        var replace4C = "Replace4C".TrueFalse();
        Creator.Replace(item4).With(replace4A, replace4B, replace4C).In(checklist);
        Assert.Equal(8, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(4, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(false));

        checklist.Simplify();
        Assert.Equal(8, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(3, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(false));
    }

    internal class QuestionTypes : ChecklistVisitor {
        internal Dictionary<QuesitonType, int> Count = new();

        internal QuestionTypes(Checklist checklist) {
            foreach (var type in Enum.GetValues(typeof(QuesitonType)).Cast<QuesitonType>())
                Count[type] = 0;
            checklist.Accept(this);
        }

        public void Visit(BooleanItem item,
            string question,
            bool? value,
            Dictionary<Person, List<Operation>> operations) =>
            Count[BooleanQuestion] += 1;

        public void Visit(MultipleChoiceItem item,
            string question,
            object? value,
            Dictionary<Person, List<Operation>> operations) =>
            Count[MultipleChoiceQuestion] += 1;

        public void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) =>
            Count[ConditionalQuestion] += 1;

        public void PreVisit(NotItem item, Item negatedItem) => Count[NotQuestion] += 1;
        public void PreVisit(OrItem item, Item item1, Item item2) => Count[OrQuestion] += 1;
        public void PreVisit(GroupItem item, List<Item> childItems) => Count[GroupQuestion] += 1;
    }

    internal enum QuesitonType {
        BooleanQuestion,
        MultipleChoiceQuestion,
        GroupQuestion,
        ConditionalQuestion,
        OrQuestion,
        NotQuestion,
    }
}