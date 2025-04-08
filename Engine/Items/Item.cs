using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items {
    public abstract class Item {
        protected readonly Dictionary<Person, List<Operation>> Operations = [];
        protected History? _history;

        internal History History() => _history
                                      ?? throw new InvalidOperationException("History hasn't been initialized");

        internal virtual void History(History history) => _history = history;

        public Item I(int firstIndex, params int[] rest) {
            var results = rest.ToList();
            results.Insert(0, firstIndex);
            return I(results);
        }

        internal abstract ItemStatus Status();

        internal abstract void Accept(ChecklistVisitor visitor);

        internal virtual void AddPerson(Person person, Role role) {
            if (!Operations.ContainsKey(person)) Operations[person] = [];
            Operations[person] = role.Operations.Concat(Operations[person]).ToHashSet().ToList();
        }

        internal virtual void RemovePerson(Person person) => Operations.Remove(person);

        internal virtual void AddOperation(Person person, List<Operation> operations) =>
            Operations[person] = operations;

        internal bool DoesAllow(Person person, Operation operation) =>
            Operations.ContainsKey(person) && Operations[person].Contains(operation);

        internal virtual bool Contains(Item desiredItem) => this == desiredItem;

        internal virtual bool Replace(Item originalItem, Item newItem) => false;

        internal virtual void Simplify() { } // Ignore by default

        internal virtual bool Remove(Item item) => false;

        internal abstract Item I(List<int> indexes);

        internal abstract List<SimpleItem> ActiveItems();
    }

    public abstract class SimpleItem : Item {
        
        internal abstract void Be(object value);

        internal abstract void Reset();
    }

    public static class ItemExtensions {
        public static NotItem Not(this Item item) => new NotItem(item);

        public static BooleanItem TrueFalse(this string question) => new BooleanItem(question);

        public static MultipleChoiceItem Choices(this string question, object firstChoice, params object[] choices) {
            if (choices.Any(choice => choice.GetType() != firstChoice.GetType()))
                throw new ArgumentException("All choices must be of the same type.");
            return new MultipleChoiceItem(question, firstChoice, choices: choices);
        }
    }
}