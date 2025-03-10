using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Engine.Tests.Unit.CarpetColor;
using Xunit;
using Engine.Persons;

namespace Engine.Tests.Unit
{
	public class MultipleChoiceItemTest
	{
		private readonly static Person creator = new Person();
		[Fact]
		public void SingleItem()
		{
			var item = "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet);
			var checklist = new Checklist( creator, item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			creator.Sets(item).To(GreenCarpet);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			creator.Sets(item).To(BlueCarpet);
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());
			creator.Reset(item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
		}
		[Fact]
		public void EmptyChecklist()
		{
			var item = "Which Carpet Color?".Choices(RedCarpet);
			var checklist = new Checklist( creator, item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			creator.Cancel(item).In(checklist);
			Assert.Equal(ChecklistStatus.NotApplicable, checklist.Status());
		}
		[Fact]
		public void MixedItems()
		{
			var item1 = "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet);
			var item2 = ("Is US citizen?").TrueFalse();
			var item3 = "Which country?".Choices("India", "Iceland", "Norway");
			var checklist = new Checklist( creator, item1, item2, item3);
			
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			creator.Sets(item1).To(GreenCarpet);
			creator.Sets(item2).To(true);
			creator.Sets(item3).To("India");
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			creator.Sets(item3).To("Poland");
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());

			var answers = new CurrentAnswers(checklist);

			Assert.Equal(GreenCarpet,answers.value("Which Carpet Color?"));
			Assert.Equal("Poland", answers.value("Which country?"));
			Assert.Equal(true, answers.value("Is US citizen?"));
			//null
			//throws
		}

        [Fact]
        public void ReplaceItem()
        {
            var item1 = "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet);
            var item2 = "Is US citizen?".TrueFalse();
            var item3 = "Which country?".Choices("India", "Iceland", "Norway");

            var checklist = new Checklist(creator, item1, item2, item3);

            var item4 = "Vehicle Type?".Choices("Car", "Bike", "Bus");
            var item5 = item2.Not();

            Assert.Equal(3, new QuestionCount(checklist).Count);

            creator.Replace(item2).With(item4, item5).In(checklist);

            Assert.Equal(4, new QuestionCount(checklist).Count);
        }

        [Fact]
        public void InsertAfterItem()
        {
            var item1 = "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet);
            var item2 = "Is US citizen?".TrueFalse();
            var item3 = "Which country?".Choices("India", "Iceland", "Norway");

            var checklist = new Checklist(creator, item1, item2, item3);

            Assert.Equal(3, new QuestionCount(checklist).Count);

            var item4 = "Vehicle Type?".Choices("Car", "Bike", "Bus");
            var item5 = "Has License?".TrueFalse();
            var item6 = "Stationery?".Choices("Pen", "Pencil");

            var item56 = item5.Or(item6);

            creator.Insert(item4, item56).After(item2).In(checklist);

            Assert.Equal(6, new QuestionCount(checklist).Count);
        }




        internal class QuestionCount : ChecklistVisitor
        {
            internal int Count;

            public QuestionCount(Checklist checklist)
            {
                checklist.Accept(this);
            }

            public void Visit(BooleanItem item, string question, bool? value, Dictionary<Person, List<Operation>> operations) =>
                Count++;

            public void Visit(MultipleChoiceItem item, string question, object? value, Dictionary<Person, List<Operation>> operations) =>
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
