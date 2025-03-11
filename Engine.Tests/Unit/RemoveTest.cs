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
    private Item failItem1B1;
    private Item failItem1B2;
    private Item failItem1A1;
    private Item failItem1A2;
    private Item failItem1;
    private Item compositeItem;
    private Item lastItem;
    private Checklist checklist;
    private Item failItem1ANot;

    public RemoveTest(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;

        firstItem = "First simple item".TrueFalse();
        baseItem1 = new BooleanItem("First condition");
        baseItem2 = new BooleanItem("Second condition");
        successItem2 = new BooleanItem("Second success leg");
        failItem2 = new BooleanItem("Second failure leg");
        successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
        failItem1A1 = new BooleanItem("First Or of first failure leg");
        failItem1A2 = new BooleanItem("First Or of first failure leg");
        failItem1A = new GroupItem(failItem1A1, failItem1A2);
        failItem1B1 = new BooleanItem("Second Or of first failure leg");
        failItem1B2 = new BooleanItem("Second Or of first failure leg");
        failItem1B = new GroupItem(failItem1B1, failItem1B2);
        failItem1ANot = failItem1A.Not();
        failItem1 = failItem1ANot.Or(failItem1B);

        compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
        lastItem = "Last simple item".TrueFalse();
        checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
    }

    [Fact]
    public void RemoveInGroup()
    {
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
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(baseItem2).From(checklist));
    }

    [Fact]
    public void RemoveOneLegInConditional()
    {
        Creator.Remove(failItem2).From(checklist);
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(successItem2).From(checklist));
    }
    
    [Fact]
    public void RemoveOneLegInOr()
    {
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(failItem1B).From(checklist));
        Creator.Remove(failItem1B1).From(checklist);
    }
    
    [Fact]
    public void CannotRemoveANotItem()
    {
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(failItem1ANot).From(checklist));
        Creator.Remove(failItem1A1).From(checklist);
    }
}
