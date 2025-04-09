using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.PrettyPrint;
using static Engine.Persons.Role;
using static Engine.Items.PrettyPrint.PrettyPrintOptions;

namespace Engine.Items {
    public class Checklist {
        internal Item _item;
        private readonly Person _creator;
        private readonly History _history = new([]);

        // Create with a Creator person only with extension method
        internal Checklist(Person creator, Item firstItem,History? history=null, params Item[] items) {
            _item = (items.Length == 0) ? firstItem : new GroupItem(firstItem, items);
            _creator = creator;
            _history = history ?? _history;
            _item.AddPerson(_creator, Creator);
            _item.History(_history);
        }

        public void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _creator, _history);
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
            && this._history.Equals(other._history);

        public override int GetHashCode() => _creator.GetHashCode()*37 + _history.GetHashCode();
        public void Replace(Item originalItem, Item newItem) {
            newItem.AddPerson(_creator, Creator);
            newItem.History(_history);
            if (_item == originalItem) {
                _item = newItem;
                return;
            }

            if (!_item.Replace(originalItem, newItem))
                throw new InvalidOperationException("Item not found in checklist");
        }

        public void Simplify() {
            _item.Simplify();
        }

        internal void Remove(Item item) {
            if (item == _item) throw new InvalidOperationException("Cannot remove the only item in the checklist");
            if (!_item.Remove(item)) throw new InvalidOperationException("Item not found in checklist");
        }

        public Item I(int firstIndex, params int[] rest) {
            if (firstIndex != 0) throw new InvalidOperationException(
                "There is only one item at the root of the Checklist hierarchy, so use index 0.");
            var indexes = rest.ToList();
            indexes.Insert(0, firstIndex);
            return _item.I(indexes);
        }

        public string ToMemento() => new ChecklistSerializer(this).Result;

        public static Checklist FromMemento(string memento) => 
            new ChecklistDeserializer(memento).Result;

        public List<SimpleItem> ActiveItems() => _item.ActiveItems();
    }
   
}