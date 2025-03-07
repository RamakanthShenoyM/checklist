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

	}
	internal enum CarpetColor
	{
		RedCarpet,
		GreenCarpet,
		NoCarpet,
		BlueCarpet
	}
}
