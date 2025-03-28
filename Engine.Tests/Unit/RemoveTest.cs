using Engine.Items;
using Xunit;
using Engine.Persons;
using Xunit.Abstractions;
using System;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit;

public class RemoveTest {
    private static readonly Person Creator = new Person(0, 0);
    private readonly ITestOutputHelper testOutput;

    private Item firstItem;
    private Item conditionItem1;
    private Item conditionItem2;
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
    private Item condition1;
    private Item lastItem;
    private Checklist checklist;
    private Item failItem1ANot;

    public RemoveTest(ITestOutputHelper testOutput) {
        this.testOutput = testOutput;

        checklist = Creator.Checklist(
            "First simple item".TrueFalse(),  // 0.0
            Conditional(                      // 0.1
                condition: "First condition".TrueFalse(),  // 0.1.0
                onSuccess: Conditional(                    // 0.1.1
                    condition: "Second condition".TrueFalse(),  // 0.1.1.0
                    onSuccess: "Second success leg".TrueFalse(), // 0.1.1.1
                    onFailure: "Second failure leg".TrueFalse()  // 0.1.1.2
                ),
        onFailure: Or(                                      // 0.1.2
                    Not(                                    // 0.1.2.0
                        Group(                                  // 0.1.2.0.0
                            "First Or of first failure leg".TrueFalse(), // 0.1.2.0.0.0
                            "First Or of first failure leg".TrueFalse()  // 0.1.2.0.0.1
                        )
                    ),
                    Group(                                  // 0.1.2.1
                        "Second Or of first failure leg".TrueFalse(),  // 0.1.2.1.0
                        "Second Or of first failure leg".TrueFalse()   // 0.1.2.1.1
                    )
                )
            ),
            "Last simple item".TrueFalse()  // 0.2
        );
        firstItem = checklist.I(0, 0);
        condition1 = checklist.I(0, 1);
        conditionItem1 = checklist.I(0, 1, 0);
        successItem1 = checklist.I(0, 1, 1); // aka condition2
        conditionItem2 = checklist.I(0, 1, 1, 0);
        successItem2 = checklist.I(0, 1, 1, 1);
        failItem2 = checklist.I(0, 1, 1, 2);
        failItem1 = checklist.I(0, 1, 2);  // the Or
        failItem1ANot = checklist.I(0, 1, 2, 0);
        failItem1A = checklist.I(0, 1, 2, 0, 0);
        failItem1A1 = checklist.I(0, 1, 2, 0, 0, 0);;
        failItem1A2 = checklist.I(0, 1, 2, 0, 0, 1);
        failItem1B = checklist.I(0, 1, 2, 1);
        failItem1B1 = checklist.I(0, 1, 2, 1, 0);
        failItem1B2 = checklist.I(0, 1, 2, 1, 1);
        lastItem = checklist.I(0, 2);
    }

    [Fact]
    public void RemoveInGroup() {
        testOutput.WriteLine(checklist.ToString());
        Creator.Remove(firstItem).From(checklist);
    }

    [Fact]
    public void RemoveLastInGroup() {
        Creator.Remove(firstItem).From(checklist);
        Creator.Remove(condition1).From(checklist);
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(lastItem).From(checklist));
    }

    [Fact]
    public void RemoveBaseInConditional() {
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(conditionItem2).From(checklist));
    }

    [Fact]
    public void RemoveOneLegInConditional() {
        Creator.Remove(failItem2).From(checklist);
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(successItem2).From(checklist));
    }

    [Fact]
    public void RemoveOneLegInOr() {
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(failItem1B).From(checklist));
        Creator.Remove(failItem1B1).From(checklist);
    }

    [Fact]
    public void CannotRemoveANotItem() {
        Assert.Throws<InvalidOperationException>(() => Creator.Remove(failItem1ANot).From(checklist));
        Creator.Remove(failItem1A1).From(checklist);
    }

    [Fact]
    public void RemoveMultipleInstances() {
        var target = "Item to remove".TrueFalse();
        var item2 = "Second item".TrueFalse();
        var item3 = "Third item".TrueFalse();
        var baseItem = "Base condition".TrueFalse();
        var successItem = "Success condition".TrueFalse();
        var conditional = new ConditionalItem(baseItem, successItem, target);
        var checklist = new Checklist(Creator, target, item2, item3, conditional);
        var replacement = "Replacement".TrueFalse();
        Creator.Remove(target).From(checklist);
        testOutput.WriteLine(checklist.ToString(false));
        Assert.Throws<ArgumentException>(() => new CurrentAnswers(checklist).Value("Item to remove"));
    }
}