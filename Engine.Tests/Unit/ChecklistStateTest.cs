using Engine.Items;
using Engine.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Engine.Tests.Unit.CarpetColor;
namespace Engine.Tests.Unit
{
	public class ChecklistStateTest
	{
		private readonly static Person creator = new Person();
		[Fact]
		public void MixedItems()
		{
			var item1 = new MultipleChoiceItem(RedCarpet, GreenCarpet, NoCarpet);
			var item2 = new BooleanItem("Is US citizen?");
			var item3 = new MultipleChoiceItem("India", "Iceland", "Norway");
			var checklist = new Checklist( creator, item1, item2, item3);
			Assert.Equal(new List<Item> { item1,item2,item3 }, checklist.Unknowns());
			Assert.Equal(new List<Item>(), checklist.Successes());
			Assert.Equal(new List<Item>(), checklist.Failures());
			creator.Sets(item2).To(true);
			creator.Sets(item3).To("India");
			Assert.Equal(new List<Item> { item1 }, checklist.Unknowns());
			Assert.Equal(new List<Item> { item2,item3 }, checklist.Successes());
			Assert.Equal(new List<Item>(), checklist.Failures());

		}
	}
}
