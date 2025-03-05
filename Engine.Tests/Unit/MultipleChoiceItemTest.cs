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
		private readonly static Creator creator = new Creator();
		[Fact]
		public void SingleItem()
		{
			var item = new MultipleChoiceItem(RedCarpet, GreenCarpet, NoCarpet);
			var checklist = new Checklist( creator, item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			item.Be(GreenCarpet);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			item.Be(BlueCarpet);
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());
			item.Reset();
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
		}
		[Fact]
		public void EmptyChecklist()
		{
			var item = new MultipleChoiceItem(RedCarpet);
			var checklist = new Checklist( creator, item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			checklist.Cancel(item);
			Assert.Equal(ChecklistStatus.NotApplicable, checklist.Status());
		}
		[Fact]
		public void MixedItems()
		{
			var item1 = new MultipleChoiceItem(RedCarpet, GreenCarpet, NoCarpet);
			var item2 = new BooleanItem();
			var item3 = new MultipleChoiceItem("India", "Iceland", "Norway");
			var checklist = new Checklist( creator, item1, item2, item3);

			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			item1.Be(GreenCarpet);
			item2.Be(true);
			item3.Be("India");
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			item3.Be("Poland");
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());
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
