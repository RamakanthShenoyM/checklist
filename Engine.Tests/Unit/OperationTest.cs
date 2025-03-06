using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Engine.Persons;
using static Engine.Persons.Operation;
using static Engine.Persons.Role;

namespace Engine.Tests.Unit
{
	public class OperationTest
	{
		private static readonly Person _creator = new Person();
		private static readonly Person _owner = new Person();
		[Fact]
		public void SingleItem()
		{
			var item1 = new BooleanItem();
			var item2 = new BooleanItem();
			var checklist = new Checklist(_creator, item1, item2);
			
			Assert.True(_creator.Can(View).On(item1));

			_creator.Add(_owner).As(Owner).To(item1);

			Assert.True(_owner.Can(View).On(item1));
			Assert.False(_owner.Can(Cancel).On(item1));
		}
	}
}
