using Engine.Items;
using Xunit.Sdk;
using Xunit;
using Engine.Persons;
using Xunit.Abstractions;
using System;

namespace Engine.Tests.Unit;

public class RemoveTest
{
    private static readonly Person Creator = new Person();
    private readonly ITestOutputHelper testOutput;

    private Item firstItem;
    private Item baseItem1;
    private Item baseItem2;    
    private Item successItem2;
    private Item failItem2;
    private Item successItem1;
    private Item failItem1A;
    private Item failItem1B;
    private Item failItem1;
    private Item compositeItem;
    private Item lastItem;
    private Checklist checklist;

    public RemoveTest(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;

        firstItem = "First simple item".TrueFalse();
        baseItem1 = new BooleanItem("First condition");
        baseItem2 = new BooleanItem("Second condition");
        successItem2 = new BooleanItem("Second success leg");
        failItem2 = new BooleanItem("Second failure leg");
        successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
        failItem1A = new BooleanItem("First Or of first failure leg");
        failItem1B = new BooleanItem("Second Or of first failure leg");
        failItem1 = failItem1A.Not().Or(failItem1B);
        compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
        lastItem = "Last simple item".TrueFalse();
        checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
    }

    [Fact]
    public void RemoveInGroup()
    {
        Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
        testOutput.WriteLine(checklist.ToString());
        Creator.Remove(firstItem).From(checklist);
    }

    [Fact]
    public void RemoveLastInGroup()
    {
        Creator.Remove(firstItem).From(checklist);
        Creator.Remove(compositeItem).From(checklist);
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(lastItem).From(checklist));
    }

    [Fact]
    public void RemoveBaseInConditional()
    {
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(baseItem1).From(checklist));
    }
}
