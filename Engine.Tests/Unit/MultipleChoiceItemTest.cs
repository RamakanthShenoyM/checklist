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
		private static readonly Person Creator = new Person(0, 0);
		
		[Fact]
		public void SingleItem() {
			var checklist = Creator.Checklist(
				"Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet)
			);
			var item = checklist.I(0);
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
		}

		[Fact]
		public void MixedItems() {
			var checklist = Creator.Checklist(
				"Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet),
				("Is US citizen?").TrueFalse(),
				"Which country?".Choices("India", "Iceland", "Norway")
			);
			var item1 = checklist.I(0, 0);
			var item2 = checklist.I(0, 1);
			var item3 = checklist.I(0, 2);
			
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			Creator.Sets(item1).To(GreenCarpet);
			Creator.Sets(item2).To(true);
			Creator.Sets(item3).To("India");
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			Creator.Sets(item3).To("Poland");
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());

			var answers = new CurrentAnswers(checklist);
			Assert.Equal(GreenCarpet,answers.Value("Which Carpet Color?"));
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

            public void Visit(BooleanItem item, Guid id, string question, bool? value, Dictionary<Person, List<Operation>> operations) =>
                Count++;

            public void Visit(MultipleChoiceItem item,Guid id, string question, object? value, List<object> choices, Dictionary<Person, List<Operation>> operations) =>
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
