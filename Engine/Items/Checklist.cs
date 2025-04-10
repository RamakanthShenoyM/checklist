using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Persons.Role;
using static Engine.Items.PrettyPrintOptions;

namespace Engine.Items {
    public class Checklist {
        internal Item _item;
        private readonly Guid _id;
        private List<Item> _checklist;
        private readonly Person _creator;
        private readonly History _history = new([]);

        // Create with a Creator person only with extension method
        internal Checklist(Person creator, Item firstItem, History? history=null, Guid? id=null, params Item[] items) {
            _item = (items.Length == 0) ? firstItem : new GroupItem(firstItem, items);
            _creator = creator;
            _id =  id??Guid.NewGuid();
            _history = history ?? _history;
            _item.AddPerson(_creator, Creator);
            _item.History(_history);
            new ChecklistIndexer(this);
        }

        public void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _creator, _history, _id);
            _history.Accept(visitor);
            _item.Accept(visitor);
            visitor.PostVisit(this, _creator, _history);
        }

        public ChecklistStatus Status() {
            if (_item.Status() == ItemStatus.Succeeded)
                return ChecklistStatus.Succeeded;
            if (_item.Status() == ItemStatus.Failed)
                return ChecklistStatus.Failed;
            return ChecklistStatus.InProgress;
        }

        internal bool HasCreator(Person person) => person == _creator;

        public override string ToString() => ToString(Full);

        public string ToString(PrettyPrintOptions option) => new PrettyPrint(this, option).Result();

        public override bool Equals(object? obj) => this == obj || obj is Checklist other && this.Equals(other);

        private bool Equals(Checklist other) =>
            this._item.Equals(other._item) 
            && this._creator.Equals(other._creator) 
            && this._history.Equals(other._history)
            && this._id.Equals(other._id);

        public override int GetHashCode() => _creator.GetHashCode()*37 + _history.GetHashCode()+ _id.GetHashCode();
        public void Replace(Item originalItem, Item newItem) {
            newItem.AddPerson(_creator, Creator);
            newItem.History(_history);
            if (_item == originalItem) {
                _item = newItem;
                new ChecklistIndexer(this);
                return;
            }

            if (!_item.Replace(originalItem, newItem))
                throw new InvalidOperationException("Item not found in checklist");
            new ChecklistIndexer(this);
        }

        public void Simplify() {
            _item.Simplify();
        }

        internal void Remove(Item item) {
            if (item == _item) throw new InvalidOperationException("Cannot remove the only item in the checklist");
            if (!_item.Remove(item)) throw new InvalidOperationException("Item not found in checklist");
            new ChecklistIndexer(this);
        }

        public Item P(int firstIndex, params int[] rest) {
            if (firstIndex != 0) throw new InvalidOperationException(
                "There is only one item at the root of the Checklist hierarchy, so use index 0.");
            var indexes = rest.ToList();
            indexes.Insert(0, firstIndex);
            return _item.P(indexes);
        }

        public string ToMemento() => new ChecklistSerializer(this).Result;

        public static Checklist FromMemento(string memento) => 
            new ChecklistDeserializer(memento).Result;

        public List<SimpleItem> ActiveItems() => _item.ActiveItems();

        public Checklist Clone() => new(_creator, _item.Clone(), _history.Clone(), _id);
    }
   
}