using CommonUtilities.Util;
using Engine.Persons;
using static Engine.Items.PrettyPrintOptions;

namespace Engine.Items {
    // Understands a text rendering of a Checklist hierarchy
    internal class PrettyPrint : ChecklistVisitor {
        private readonly PrettyPrintOptions _option;
        private int _indentionLevel;
        private readonly List<Triple> _conditionalItems = [];
        private String _result = "";
        private readonly HashSet<Item> _extraIndentedItems = new();

        internal PrettyPrint(Checklist checklist, PrettyPrintOptions option = Full) {
            _option = option;
            checklist.Accept(this);
        }

        public void PreVisit(Checklist checklist, Person creator, History history) {
            _result += String.Format("{0}Checklist created by {1}\n", Indention, creator);
            _indentionLevel++;
        }

        public void PostVisit(Checklist checklist, Person creator, History history) {
            _indentionLevel--;
        }

        public void Visit(BooleanItem item,
            Guid id,
            Position position,
            string question,
            bool? value,
            Dictionary<Person, List<Operation>> operations,
            History history) {
            LabelIndention(item);
            _result += String.Format("{0}Question: [{1}] {2} Value: {3}\n", Indention, position, question, Format(value));
            OperationsDescription(operations);
            LabelUndention(item);
        }

        public void Visit(MultipleChoiceItem item,
            Guid id,
            Position position,
            string question,
            object? value,
            List<object> choices,
            Dictionary<Person, List<Operation>> operations,
            History history) {
            LabelIndention(item);
            _result += String.Format("{0}Boolean Item with value {1}\n", Indention, Format(value));
            OperationsDescription(operations);
            LabelUndention(item);
        }

        public void PreVisit(ConditionalItem item,
            Position position,
            Item baseItem,
            Item? successItem,
            Item? failureItem,
            Dictionary<Person, List<Operation>> operations) {
            LabelIndention(item);
            _result += String.Format("{0}Conditional [{1}]\n", Indention, position);
            _conditionalItems.Add(new Triple(baseItem, successItem, failureItem));
            _indentionLevel++;
        }

        public void PostVisit(ConditionalItem item,
            Position position,
            Item baseItem,
            Item? successItem,
            Item? failureItem,
            Dictionary<Person, List<Operation>> operations) {
            _indentionLevel--;
            LabelUndention(item);
        }

        public void PreVisit(OrItem item,
            Position position,
            Item item1,
            Item item2,
            Dictionary<Person, List<Operation>> operations) {
            LabelIndention(item);
            _result += String.Format("{0}Either/Or [{1}]\n", Indention, position);
            _indentionLevel++;
        }

        public void PostVisit(OrItem item,
            Position position,
            Item item1,
            Item item2,
            Dictionary<Person, List<Operation>> operations) {
            _indentionLevel--;
            LabelUndention(item);
        }

        public void PreVisit(NotItem item,
            Position position,
            Item negatedItem,
            Dictionary<Person, List<Operation>> operations) {
            LabelIndention(item);
            _result += String.Format("{0}Not (the following) [{1}]\n", Indention, position);
            _indentionLevel++;
        }

        public void PostVisit(NotItem item,
            Position position,
            Item negatedItem,
            Dictionary<Person, List<Operation>> operations) {
            _indentionLevel--;
            LabelUndention(item);
        }

        public void PreVisit(GroupItem item,
            Position position,
            List<Item> childItems,
            Dictionary<Person, List<Operation>> operations) {
            LabelIndention(item);
            _result += String.Format("{0}Group of Items [{1}]\n", Indention, position);
            _indentionLevel++;
        }

        public void PostVisit(GroupItem item,
            Position position,
            List<Item> childItems,
            Dictionary<Person, List<Operation>> operations) {
            _indentionLevel--;
            LabelUndention(item);
        }

        private void OperationsDescription(Dictionary<Person, List<Operation>> operations) {
            if (_option == NoOperations) return;
            _indentionLevel++;
            foreach (var operation in operations) {
                _result += String.Format(
                    "{0}{1} operations are: {2}\n",
                    Indention,
                    operation.Key,
                    Operations(operation.Value));
            }

            _indentionLevel--;
        }

        private string Indention => new(' ', _indentionLevel * 2);

        private string Format(object? value) => value?.ToString() ?? "<unknown>";

        private string Operations(List<Operation> operations) =>
            string.Join(",", operations.Select(o => Format(o)).ToArray());

        private void LabelIndention(Item item) {
            if (_conditionalItems.Count == 0) return;
            var label = Triple.Matches(_conditionalItems, item);
            if (label.Length == 0) return;
            _extraIndentedItems.Add(item);
            _result += $"{Indention}{label}\n";
            _indentionLevel++;
        }

        private void LabelUndention(Item item) {
            if (_extraIndentedItems.Remove(item)) _indentionLevel--;
        }

        public string Result() => _result;

        private class Triple {
            private Item? _baseItem;
            private Item? _successItem;
            private Item? _failItem;

            internal Triple(Item? baseItem, Item? successItem, Item? failItem) {
                _baseItem = baseItem;
                _successItem = successItem;
                _failItem = failItem;
            }

            internal static string Matches(List<Triple> triples, Item item) {
                foreach (var triple in triples) {
                    var result = triple.Matches(item);
                    if (result.Length > 0) return result;
                }

                return "";
            }

            internal string Matches(Item item) {
                if (item == _baseItem) {
                    _baseItem = null;
                    return "Conditional Item:";
                }

                if (item == _successItem) {
                    _successItem = null;
                    return "Success Leg:";
                }

                if (item == _failItem) {
                    _failItem = null;
                    return "Failure Leg:";
                }

                return "";
            }
        }
    }

    public enum PrettyPrintOptions {
        Full, NoOperations
    }
}