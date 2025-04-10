﻿using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items {
    public abstract class Item {
        protected readonly Dictionary<Person, List<Operation>> Operations = [];
        private History? _history; // Shadow reference to Checklist History
        protected Position? _position; // Established when creating a checklist or modigying one

        internal Position Position() =>
            _position ?? throw new InvalidOperationException("Position has not been initialized");

        internal void Position(Position position) => _position = position;

        internal History History() =>
            _history ?? throw new InvalidOperationException("History hasn't been initialized");

        internal virtual void History(History history) => _history = history;

        internal Item P(Position position) {
            if (position.Equals(Position())) return this;
            var subItem = SubItems().Find(subItem => subItem.Position().IsPartialMatch(position))
                ?? throw new ArgumentException($"No item exists at position {position}");
            return subItem.P(position);
        }

        protected abstract List<Item> SubItems();

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

        internal abstract List<SimpleItem> ActiveItems();

        internal abstract Item Clone();
    }

    public abstract class SimpleItem : Item {
        internal abstract void Be(object value);

        internal abstract void Reset();

        protected override List<Item> SubItems() => [];
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