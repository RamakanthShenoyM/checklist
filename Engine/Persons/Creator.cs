using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Persons
{
	public class Creator
	{
		public Permission Can(Persons.Roles view)
		{
			return new Permission(this, view);
		}

		public class Permission
		{
			private readonly Creator _creator;
			private readonly Roles _role;

			internal Permission(Creator creator, Roles role)
			{
				this._creator = creator;
				this._role = role;
			}

			public bool On(Item item)
			{
				throw new NotImplementedException();
			}
		}
	}
}
