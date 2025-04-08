using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Items;
using Engine.Persons;
using Xunit;
using Xunit.Abstractions;
using static Engine.Tests.Unit.GroupMergeTest.QuesitonType;
using static Engine.Items.PrettyPrintOptions;
using CommonUtilities.Util;

namespace Engine.Tests.Unit;

// Ensures that GroupItems can be merged
public class GroupMergeTest(ITestOutputHelper testOutput) {
    private static readonly Person Creator = new(0, 0);

    [Fact]
    public void SingleGroupReplace() {
        var checklist = Creator.Checklist(
            "Item1".TrueFalse(),
            "Item2".TrueFalse(),
            "Item3".TrueFalse());
        Assert.Equal(3, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(1, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(NoOperations));

        var replace2A = "Replace2A".TrueFalse();
        var replace2B = "Replace2B".TrueFalse();
        var replace2C = "Replace2C".TrueFalse();
        Creator.Replace(checklist.P(0, 1)).With(replace2A, replace2B, replace2C).In(checklist);
        Assert.Equal(5, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(2, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(NoOperations));

        checklist.Simplify();
        Assert.Equal(5, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(1, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(NoOperations));
    }

    [Fact]
    public void MultipleGroupReplace() {
        var checklist = Creator.Checklist(
            "Item1".TrueFalse(),
            "Item2".TrueFalse(),
            "Item3".TrueFalse(),
            "Item4".TrueFalse());
        Assert.Equal(4, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(1, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(NoOperations));

        Creator.Replace(checklist.P(0, 1)).With(
            "Replace2A".TrueFalse(), 
            "Replace2B".TrueFalse(), 
            "Replace2C".TrueFalse()
            ).In(checklist);
        Assert.Equal(6, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(2, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(NoOperations));

        Creator.Replace(checklist.P(0, 3)).With(
            "Replace4A".TrueFalse(), 
            "Replace4B".TrueFalse(), 
            "Replace4C".TrueFalse()
            ).In(checklist);
        Assert.Equal(8, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(3, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(NoOperations));

        checklist.Simplify();
        Assert.Equal(8, new QuestionTypes(checklist).Count[BooleanQuestion]);
        Assert.Equal(1, new QuestionTypes(checklist).Count[GroupQuestion]);
        testOutput.WriteLine(checklist.ToString(NoOperations));
    }

    internal class QuestionTypes : ChecklistVisitor {
        internal Dictionary<QuesitonType, int> Count = new();

        internal QuestionTypes(Checklist checklist) {
            foreach (var type in Enum.GetValues(typeof(QuesitonType)).Cast<QuesitonType>())
                Count[type] = 0;
            checklist.Accept(this);
        }

        public void Visit(BooleanItem item,
            Guid id,
            Position position,
            string question,
            bool? value,
            Dictionary<Person, List<Operation>> operations,
            History history) =>
            Count[BooleanQuestion] += 1;

        public void Visit(MultipleChoiceItem item,
            Guid id,
            Position position,
            string question,
            object? value,
            List<object> choices,
            Dictionary<Person, List<Operation>> operations,
            History history) =>
            Count[MultipleChoiceQuestion] += 1;

        public void PreVisit(ConditionalItem item,
            Position position,
            Item baseItem,
            Item? successItem,
            Item? failureItem,
            Dictionary<Person, List<Operation>> operations) =>
            Count[ConditionalQuestion] += 1;

        public void PreVisit(NotItem item,
            Position position,
            Item negatedItem,
            Dictionary<Person, List<Operation>> operations) => Count[NotQuestion] += 1;
        public void PreVisit(OrItem item,
            Position position,
            Item item1,
            Item item2,
            Dictionary<Person, List<Operation>> operations) => Count[OrQuestion] += 1;
        public void PreVisit(GroupItem item,
            Position position,
            List<Item> childItems,
            Dictionary<Person, List<Operation>> operations) => Count[GroupQuestion] += 1;
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