using Engine.Persons;
using static Engine.Persons.Role;

namespace Engine.Items
{
	public class Checklist
	{
		private readonly List<Item> _items;
		private readonly Person _creator;

		public Checklist(Person creator, Item firstItem, params Item[] items)
		{
			_items = items.ToList();
			_items.Insert(0, firstItem);
			_creator = creator;
			_items.ForEach(item => item.AddPerson(_creator, Creator));	
		}

		public void Accept(ChecklistVisitor visitor) {
			visitor.PreVisit(this, _creator);
			foreach (var item in _items) item.Accept(visitor);
			visitor.PostVisit(this, _creator);
		}

        internal void Add(params Item[] items)
        {
            items.ToList().ForEach(item => item.AddPerson(_creator, Creator));
            _items.AddRange(items);
        }

        internal void Replace(Item itemToBeReplaced, Item[] items)
        {
            items.ToList().ForEach(item => item.AddPerson(_creator, Creator));
            var index = _items.IndexOf(itemToBeReplaced);
            _items.RemoveAt(index);
            _items.InsertRange(index, items);
        }
        internal void InsertAfter(Item insertAfterItem, Item[] items)
        {
            items.ToList().ForEach(item => item.AddPerson(_creator, Creator));
            var index = _items.IndexOf(insertAfterItem);
            _items.InsertRange(index + 1, items);
        }

        internal void Cancel(Item item) => _items.Remove(item);

		public List<Item> Failures() => _items.FindAll(item => item.Status() == ItemStatus.Failed);
		
		public ChecklistStatus Status()
		{
			if (_items.Count == 0) return ChecklistStatus.NotApplicable;
			var statuses = _items.Select(item => item.Status()).ToList();
			if (statuses.All(status => status == ItemStatus.Succeeded))
				return ChecklistStatus.Succeeded;
			if (statuses.Any(status => status == ItemStatus.Failed))
				return ChecklistStatus.Failed;
			return ChecklistStatus.InProgress;
		}

		public List<Item> Successes() => _items.FindAll(item => item.Status() == ItemStatus.Succeeded);

		public List<Item> Unknowns() => _items.FindAll(item => item.Status() == ItemStatus.Unknown);
		
		internal bool Contains(Item desiredItem) => _items.Any(item => item.Contains(desiredItem));

        internal bool HasCreator(Person person) => person == _creator;

        public override string ToString() => ToString(true);

        public string ToString(bool showOperations) => new PrettyPrint(this, showOperations).Result();

        public void Replace(Item originalItem, Item newItem)
        {
			newItem.AddPerson(_creator,Creator);
            if (_items.Contains(originalItem))
            {
                var index = _items.IndexOf(originalItem);
                _items.RemoveAt(index);
                _items.Insert(index, newItem);
				return;
            }
            if (!_items.Any(item => item.Replace(originalItem, newItem))) throw new InvalidOperationException("Item not found in checklist");
        }

        public void Simplify() {
	        foreach (var item in _items) item.Simplify();
        }
	}
}
