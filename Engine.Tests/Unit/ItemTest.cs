using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Engine.Tests.Unit
{
	public class ItemTest
	{
		[Fact]
		public void SingleItem()
		{
			var item = new BooleanItem();
			var checklist = new Checklist(item);
			Assert.Equal(ChecklistStatus.InProgress,checklist.Status());
			item.Be(true);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			item.Be(false);
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());
			item.Reset();
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
		}

		[Fact]
		public void MultipleItems()
		{
			var item1 = new BooleanItem();
			var item2 = new BooleanItem();
			var item3 = new BooleanItem();
			var checklist = new Checklist(item1,item2,item3);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			item1.Be(true);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());

			item2.Be(true);
			item3.Be(true);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			item2.Be(false);
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());

			item1.Reset();
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());
			item2.Reset();
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());

		}
	}
}
