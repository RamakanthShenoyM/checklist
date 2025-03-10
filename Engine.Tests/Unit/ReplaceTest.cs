using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Persons;
using Xunit;
using static Engine.Tests.Unit.CarpetColor;
using Xunit.Sdk;
using Xunit.Abstractions;


namespace Engine.Tests.Unit
{
    public class ReplaceTest(ITestOutputHelper testOutput)
    {
        private static readonly Person Creator = new Person();
        [Fact]
        public void ReplaceItem()
        {
            var item1 = "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet);
            var item2 = "Is US citizen?".TrueFalse();
            var item3 = "Which country?".Choices("India", "Iceland", "Norway");

            var checklist = new Checklist(Creator, item1, item2, item3);

            var item4 = "Vehicle Type?".Choices("Car", "Bike", "Bus");
            var item5 = item2.Not();

            Assert.Equal(3, new MultipleChoiceItemTest.QuestionCount(checklist).Count);

            Creator.Replace1(item2).With(item4, item5).In(checklist);

            Assert.Equal(4, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
        }

        [Fact]
        public void ConditionalWithConditional()
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
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8,new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            var replace1 = "Replace1".TrueFalse();
            var replace2 = "Replace2".TrueFalse();
            Creator.Replace1(successItem2).With(replace1,replace2).In(checklist);
            Assert.Equal(9, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
        }

        [Fact]
        public void ReplacingConditionalBase()
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
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            var replace1 = "Replace1".TrueFalse();
            var replace2 = "Replace2".TrueFalse();
            Creator.Replace1(baseItem2).With(replace1, replace2).In(checklist);
            Assert.Equal(9, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
        } 
        
        [Fact]
        public void ReplacingOrLeg()
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
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            var replace1 = "Replace1".TrueFalse();
            var replace2 = "Replace2".TrueFalse();
            Creator.Replace1(failItem1A).With(replace1, replace2).In(checklist);
            Assert.Equal(9, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
        }
        
        [Fact]
        public void ReplacingCompleteConditional()
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
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            var replace1 = "Replace1".TrueFalse();
            var replace2 = "Replace2".TrueFalse();
            Creator.Replace1(successItem1).With(replace1, replace2).In(checklist);
            Assert.Equal(7, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
        } 
        
        [Fact]
        public void ReplacingTopConditional()
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
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            var replace1 = "Replace1".TrueFalse();
            var replace2 = "Replace2".TrueFalse();
            var group1 = new GroupItem(replace1, replace2);
            Creator.Replace1(compositeItem).With(group1).In(checklist);
            Assert.Equal(4, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
            var replace3 = "Replace3".TrueFalse();
            var replace4 = "Replace4".TrueFalse();
            var replace5 = "Replace5".TrueFalse();
            Creator.Replace1(group1).With(replace3,replace4,replace5).In(checklist);
            Assert.Equal(5, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            testOutput.WriteLine(checklist.ToString());
        }
        
        [Fact]
        public void InsertAfterToConditional()
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
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            var addition1 = "Addition1".TrueFalse();
            var addition2 = "Addition2".TrueFalse();
            var group1 = new GroupItem(addition1, addition2);
            Creator.Insert1(addition1, addition2).After(successItem1).In(checklist);
            testOutput.WriteLine(checklist.ToString());
            Assert.Equal(10, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
        }
        
        [Fact]
        public void InsertBeforeToConditional()
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
            var lastItem = "Last simple item".TrueFalse();

            var compositeItem = new ConditionalItem(baseItem1, successItem1, failItem1);
            var checklist = new Checklist(Creator, firstItem, compositeItem, lastItem);
            Assert.Equal(8, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
            var addition1 = "Addition1".TrueFalse();
            var addition2 = "Addition2".TrueFalse();
            var group1 = new GroupItem(addition1, addition2);
            Creator.Insert1(addition1, addition2).Before(successItem1).In(checklist);
            testOutput.WriteLine(checklist.ToString());
            Assert.Equal(10, new MultipleChoiceItemTest.QuestionCount(checklist).Count);
        }

    }
}
