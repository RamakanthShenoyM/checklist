using Engine.Items;
using Xunit;
using static Engine.Items.ChecklistStatus;

namespace Engine.Tests.Unit
{
	public class CompositeItemTest
	{
		[Fact]
		public void Boolean()
		{
			var baseItem = new BooleanItem();
			var successItem = new BooleanItem();
			var failItem = new BooleanItem();

			var compositeItem = new CompositeItem(baseItem, successItem, failItem);
			var checklist = new Checklist(compositeItem);

			Assert.Equal(InProgress, checklist.Status());
			baseItem.Be(true);
			Assert.Equal(InProgress, checklist.Status());
			successItem.Be(false);
			Assert.Equal(Failed, checklist.Status());
			baseItem.Be(false);
			Assert.Equal(InProgress, checklist.Status());
			failItem.Be(true);
			Assert.Equal(Succeeded, checklist.Status());
		}
		[Fact]
		public void BooleanWithFalse()
		{
			var baseItem = new BooleanItem();
			var failItem = new BooleanItem();

			var compositeItem = new CompositeItem(baseItem, failItem: failItem);
			var checklist = new Checklist(compositeItem);

			Assert.Equal(InProgress, checklist.Status());
			baseItem.Be(true);
			Assert.Equal(Succeeded, checklist.Status());
			baseItem.Be(false);
			Assert.Equal(InProgress, checklist.Status());
			failItem.Be(true);
			Assert.Equal(Succeeded, checklist.Status());
		}
		[Fact]
		public void BooleanWithTrue()
		{
			var baseItem = new BooleanItem();
			var successItem = new BooleanItem();

			var compositeItem = new CompositeItem(baseItem, successItem: successItem);
			var checklist = new Checklist(compositeItem);

			Assert.Equal(InProgress, checklist.Status());
			baseItem.Be(false);
			Assert.Equal(Failed, checklist.Status());
			baseItem.Be(true);
			Assert.Equal(InProgress, checklist.Status());
			successItem.Be(true);
			Assert.Equal(Succeeded, checklist.Status());
		}

		[Fact]
		public void CompositeItemMultipleLevel()
		{
			var asiaItem = new MultipleChoiceItem("India","Srilanka");
			var usItem = new BooleanItem();
			var nordicItem = new MultipleChoiceItem("Iceland", "Norway","Sweden","Finland","Denmark");
			var failItem = new CompositeItem(nordicItem,usItem);
			
			var compositeItem = new CompositeItem(asiaItem, usItem, failItem);
			var checklist = new Checklist(compositeItem);

			Assert.Equal(InProgress, checklist.Status());
			asiaItem.Be("Srilanka");
			Assert.Equal(InProgress, checklist.Status());
			usItem.Be(true);
			Assert.Equal(Succeeded, checklist.Status());
			usItem.Be(false);
			Assert.Equal(Failed, checklist.Status());

			asiaItem.Be("Uganda");
			Assert.Equal(InProgress, checklist.Status());
			nordicItem.Be("Iceland");
			Assert.Equal(Failed, checklist.Status());

			failItem.Reset();
			Assert.Equal(InProgress, checklist.Status());
            asiaItem.Be("Srilanka");
            Assert.Equal(Failed, checklist.Status());
        }

	}
}
