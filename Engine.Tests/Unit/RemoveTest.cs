using Engine.Items;
using Xunit.Sdk;
using Xunit;
using Engine.Persons;
using Xunit.Abstractions;

namespace Engine.Tests.Unit;

public class RemoveTest(ITestOutputHelper testOutput)
{
    private static readonly Person Creator = new Person();

    [Fact]
    public void RemoveInGroup()
    {
        var firstItem = "First simple item".TrueFalse();
        var baseItem1 = new BooleanItem("First condition");
        var baseItem2 = new BooleanItem("Second condition");
        var successItem2 = new BooleanItem("Second success leg");
        var failItem2 = new BooleanItem("Second failure leg");
        var successItem1 = new ConditionalItem(baseItem2, successItem2, failItem2);
        var failItem1A = new BooleanItem("First Or of first failure leg");
        var failItem1B = new BooleanItem("Second Or of first failure leg");
        var failItem1 = failItem1A.Not().Or(failItem1B);
        var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
        var lastItem = "Last simple item".TrueFalse();
        var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
        Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
        testOutput.WriteLine(checklist.ToString());
        // Creator.Remove(firstItem).From(checklist);
    }
}
