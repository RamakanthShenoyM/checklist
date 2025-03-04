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
			var checklist = new Checklist(new BooleanItem());
			Assert.Equal(ChecklistStatus.InProgress,checklist.Status());
		}
	}
}
