using Engine.Items;
using Engine.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Engine.Tests.Unit
{
	public class BooleanItemTest
	{
		private readonly static Person creator = new Person();
		[Fact]
		public void SingleItem()
		{
			var item = new BooleanItem();
			var checklist = new Checklist( creator, item);
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
			var checklist = new Checklist( creator, item1,item2,item3);
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
        [Fact]
        public void CancelItem()
        {
            var item1 = new BooleanItem();
            var item2 = new BooleanItem();
            var checklist = new Checklist( creator, item1, item2);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            item1.Be(true);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            item2.Be(false);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());
			checklist.Cancel(item2);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
		}
        [Fact]
        public void ReplaceItems()
        {
            var item1 = new BooleanItem();
            var item2 = new BooleanItem();
            var item3 = new BooleanItem();
            var item4 = new BooleanItem();
            var checklist = new Checklist( creator, item1, item2);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            item1.Be(true);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            item2.Be(false);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());
            checklist.Cancel(item2);
			checklist.Add(item3, item4);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
			item4.Be(false);
            Assert.Equal(ChecklistStatus.Failed, checklist.Status());
        }
		[Fact]
        public void InvalidValue()
        {
            var item = new BooleanItem();
            var checklist = new Checklist( creator, item);
            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
            item.Be(true);
            Assert.Equal(ChecklistStatus.Succeeded, checklist.Status());
			Assert.Throws<InvalidCastException>(() => item.Be("green"));
        }
    }
}
