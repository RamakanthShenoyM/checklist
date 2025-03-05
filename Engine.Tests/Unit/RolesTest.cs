using Engine.Items;
using System;
using Engine.Persons;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Engine.Persons.Permissions;

namespace Engine.Tests.Unit
{
	public class RolesTest
	{
		[Fact]
		public void Creator()
		{
			var creator = new Creator();
			var item = new BooleanItem();
			var checklist = new Checklist(creator,item);
			Assert.True(creator.Can(View).On(item));
		}
	}
}
