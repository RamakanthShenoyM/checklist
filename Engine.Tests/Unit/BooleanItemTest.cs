using System;
using Engine.Items;
using Engine.Persons;
using Xunit;

namespace Engine.Tests.Unit
{
	public class BooleanItemTest
	{
		private static readonly Person Creator = new Person();
		[Fact]
		public void SingleItem()
		{
			var item = new BooleanItem("Is US citizen?");
			var checklist = new Checklist(Creator, item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			Creator.Sets(item).To(true);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			Creator.Sets(item).To(false);
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());
			Creator.Reset(item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			Creator.Sets(item).To(true);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
		}

		[Fact]
		public void MultipleItems()
		{
			var item1 = "Is US citizen?".TrueFalse();
			var item2 = "Is Indian citizen?".TrueFalse();
			var item3 = "Is Nordic citizen?".TrueFalse();
			var checklist = new Checklist(Creator, item1, item2, item3);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			Creator.Sets(item1).To(true);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());

			Creator.Sets(item2).To(true);
			Creator.Sets(item3).To(true);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			Creator.Sets(item2).To(false);
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());

			Creator.Reset(item1);
			Assert.Equal(ChecklistStatus.Failed, checklist.Status());
			Creator.Reset(item2);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			var answers = new CurrentAnswers(checklist);
			Assert.Null(answers["Is US citizen?"]);
			Assert.Null(answers["Is Indian citizen?"]);
			Assert.Equal(true, answers["Is Nordic citizen?"]);
		}
		
		[Fact]
		public void InvalidValue()
		{
			var item = new BooleanItem("Is US citizen?");
			var checklist = new Checklist(Creator, item);
			Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			Creator.Sets(item).To(true);
			Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			Assert.Throws<InvalidCastException>(() => Creator.Sets(item).To("green"));
		}
	}
}
