using Engine.Persons;
using static Engine.Persons.Role;

namespace Engine.Items
{
	public class Checklist
	{
		private Item _item;
		private readonly Person _creator;

		public Checklist(Person creator, Item firstItem, params Item[] items)
		{
            _item = new GroupItem(firstItem, items);
			_creator = creator;
			_item.AddPerson(_creator, Creator);	
		}

		public void Accept(ChecklistVisitor visitor) {
			visitor.PreVisit(this, _creator);
			_item.Accept(visitor);
			visitor.PostVisit(this, _creator);
		}
		
		public ChecklistStatus Status()
		{
			if (_item.Status() == ItemStatus.Succeeded)
				return ChecklistStatus.Succeeded;
			if (_item.Status() == ItemStatus.Failed)
				return ChecklistStatus.Failed;
			return ChecklistStatus.InProgress;
		}
		
		internal bool Contains(Item desiredItem) => _item.Contains(desiredItem);

        internal bool HasCreator(Person person) => person == _creator;

        public override string ToString() => ToString(true);

        public string ToString(bool showOperations) => new PrettyPrint(this, showOperations).Result();

        public void Replace(Item originalItem, Item newItem)
        {
			newItem.AddPerson(_creator,Creator);

			if (_item == originalItem)
			{
                _item = newItem; 
				return;
            }

            if (!_item.Replace(originalItem, newItem)) throw new InvalidOperationException("Item not found in checklist");
        }
        public void Simplify() {
	        _item.Simplify();
        }

        internal void Remove(Item item)
        {

            throw new NotImplementedException();
        }
    }
}
