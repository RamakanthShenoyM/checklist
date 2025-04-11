using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Persons.Role;
using static Engine.Items.PrettyPrintOptions;

namespace Engine.Items {
    public class Checklist {
        internal Item Item;
        private readonly Guid _id;
        private readonly Person _creator;
        private readonly History _history = new([]);

        // Create with a Creator person only with extension method
        internal Checklist(Person creator, Item firstItem, History? history=null, Guid? id=null, params Item[] items) {
            Item = (items.Length == 0) ? firstItem : new GroupItem(firstItem, items);
            _creator = creator;
            _id =  id??Guid.NewGuid();
            _history = history ?? _history;
            Item.AddPerson(_creator, Creator);
            Item.History(_history);
            ChecklistIndexer.Index(this);
        }

        public void Accept(ChecklistVisitor visitor) {
            visitor.PreVisit(this, _creator, _history, _id);
            _history.Accept(visitor);
            Item.Accept(visitor);
            visitor.PostVisit(this, _creator, _history);
        }

        public ChecklistStatus Status() {
            if (Item.Status() == ItemStatus.Succeeded)
                return ChecklistStatus.Succeeded;
            if (Item.Status() == ItemStatus.Failed)
                return ChecklistStatus.Failed;
            return ChecklistStatus.InProgress;
        }

        internal bool HasCreator(Person person) => Equals(person, _creator);

        public override string ToString() => ToString(Full);

        public string ToString(PrettyPrintOptions option) => new PrettyPrint(this, option).Result();

        public override bool Equals(object? obj) => this == obj || obj is Checklist other && this.Equals(other);

        private bool Equals(Checklist other) =>
            this.Item.Equals(other.Item) 
            && this._creator.Equals(other._creator) 
            && this._history.Equals(other._history)
            && this._id.Equals(other._id);

        public override int GetHashCode() => _creator.GetHashCode()*37 + _history.GetHashCode()+ _id.GetHashCode();
        public void Replace(Item originalItem, Item newItem) {
            newItem.AddPerson(_creator, Creator);
            newItem.History(_history);
            if (Item == originalItem) {
                Item = newItem;
                ChecklistIndexer.Index(this);
                return;
            }

            if (!Item.Replace(originalItem, newItem))
                throw new InvalidOperationException("Item not found in checklist");
            ChecklistIndexer.Index(this);
        }

        public void Simplify() {
            Item.Simplify();
        }

        internal void Remove(Item item) {
            if (item == Item) throw new InvalidOperationException("Cannot remove the only item in the checklist");
            if (!Item.Remove(item)) throw new InvalidOperationException("Item not found in checklist");
            ChecklistIndexer.Index(this);
        }

        public Item P(int firstIndex, params int[] rest) => P(new Position(firstIndex, rest));

        public Item P(Position position) => Item.P(position);

        public string ToMemento() => new ChecklistSerializer(this).Result;

        public static Checklist FromMemento(string memento) => 
            new ChecklistDeserializer(memento).Result;

        public List<SimpleItem> ActiveItems() => Item.ActiveItems();

        public Checklist Clone() => new(_creator, Item.Clone(), _history.Clone(), _id);
    }
   
}