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
		public void InProgress()
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
	}
}
