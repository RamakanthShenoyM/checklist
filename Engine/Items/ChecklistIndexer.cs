using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items {
    // Creates/updates the Position of each Item in a specific Checklist
    public class ChecklistIndexer : ChecklistVisitor {
        private Position _position = new();

        public ChecklistIndexer(Checklist checklist) {
            checklist.Accept(this);
        }

        public void Visit(BooleanItem item,
            Guid id,
            Position position,
            string question,
            bool? value,
            Dictionary<Person, List<Operation>> operations,
            History history) {
            item.Position(_position.Clone());
            _position.Increment();
        }

        public void Visit(MultipleChoiceItem item,
            Guid id,
            Position position,
            string question,
            object? value,
            List<object> choices,
            Dictionary<Person, List<Operation>> operations,
            History history) {
            item.Position(_position.Clone());
            _position.Increment();
        }

        public void Visit(NullItem item) {
            item.Position(_position.Clone());
            _position.Increment();
        }

        public void PreVisit(ConditionalItem item,
            Position position,
            Item baseItem,
            Item? successItem,
            Item? failureItem,
            Dictionary<Person, List<Operation>> operations) {
            item.Position(_position.Clone());
            _position.Deeper();
        }

        public void PostVisit(ConditionalItem item,
            Position position,
            Item baseItem,
            Item? successItem,
            Item? failureItem,
            Dictionary<Person, List<Operation>> operations) {
            _position.Truncate();
            _position.Increment();
        }

        public void PreVisit(OrItem item,
            Position position,
            Item item1,
            Item item2,
            Dictionary<Person, List<Operation>> operations) {
            item.Position(_position.Clone());
            _position.Deeper();
        }

        public void PostVisit(OrItem item,
            Position position,
            Item item1,
            Item item2,
            Dictionary<Person, List<Operation>> operations) {
            _position.Truncate();
            _position.Increment();
        }

        public void PreVisit(NotItem item,
            Position position,
            Item negatedItem,
            Dictionary<Person, List<Operation>> operations) {
            item.Position(_position.Clone());
            _position.Deeper();
        }

        public void PostVisit(NotItem item,
            Position position,
            Item negatedItem,
            Dictionary<Person, List<Operation>> operations) {
            _position.Truncate();
            _position.Increment();
        }

        public void PreVisit(GroupItem item,
            Position position,
            List<Item> childItems,
            Dictionary<Person, List<Operation>> operations) {
            item.Position(_position.Clone());
            _position.Deeper();
        }

        public void PostVisit(GroupItem item,
            Position position,
            List<Item> childItems,
            Dictionary<Person, List<Operation>> operations) {
            _position.Truncate();
            _position.Increment();
        }
    }
}