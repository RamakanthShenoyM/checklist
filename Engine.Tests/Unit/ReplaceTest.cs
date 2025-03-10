using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Persons;
using Xunit;
using static Engine.Tests.Unit.CarpetColor;


namespace Engine.Tests.Unit
{
    public class ReplaceTest
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
    }
}
