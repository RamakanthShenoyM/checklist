using Engine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Engine.Persons.Permissions;
namespace Engine.Persons
{
	public class Creator
	{
		private static readonly List<Permissions> _permissions = new List<Permissions> {
		View,
		AddItem,
		Cancel,
		DeleteItem,
		Set,
		Reset,
		AddOwner,
		RemoveOwner,
		AddViewer,
		RemoveViewer };

		public Permission Can(Persons.Permissions view)
		{
			return new Permission(this, view);
		}

		private bool Has(Permissions permission) => _permissions.Contains(permission);

		public class Permission
		{
			private readonly Creator _person;
			private readonly Permissions _permission;

			internal Permission(Creator person, Permissions permission)
			{
				this._person = person;
				this._permission = permission;
			}

			public bool On(Item item)
			{
				if (!item.HasPerson(_person)) return false;
				return _person.Has(_permission);
			}
		}

		
	}
}
