using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.ItemStatus;

namespace Engine.Items {
    public class NullItem : Item {
        internal override void Accept(ChecklistVisitor visitor) => visitor.Visit(this);

        internal override Item P(List<int> indexes) => this;

        internal override List<SimpleItem> ActiveItems() => [];

        protected override List<Item> SubItems() => [];

        internal override ItemStatus Status() => Succeeded;

        internal override void AddPerson(Person person, Role role) {
            //Ignore this
        }

        internal override void History(History history) {
            //Ignore this
        }

        internal override void RemovePerson(Person person) {
            //Ignore this
        }

        public override bool Equals(object? obj) => this == obj || obj is NullItem;

        public override int GetHashCode() => nameof(NullItem).GetHashCode();
    }
}