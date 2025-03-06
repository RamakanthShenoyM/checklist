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
			var item = new MultipleChoiceItem(RedCarpet, GreenCarpet, NoCarpet);
			var checklist = new Checklist( creator, item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			creator.Sets(item).To(GreenCarpet);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			creator.Sets(item).To(BlueCarpet);
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
			creator.Sets(item1).To(GreenCarpet);
			creator.Sets(item2).To(true);
			creator.Sets(item3).To("India");
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			creator.Sets(item3).To("Poland");
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
