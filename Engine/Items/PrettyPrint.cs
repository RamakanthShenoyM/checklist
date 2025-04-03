using Engine.Persons;
using static Engine.Items.PrettyPrint.PrettyPrintOptions;

namespace Engine.Items {
    // Understands a text rendering of a Checklist hierarchy
    public class PrettyPrint : ChecklistVisitor {
        private readonly PrettyPrintOptions _option;
        private int _indentionLevel;
        private readonly List<Triple> _conditionalItems = [];
        private String _result = "";
        private readonly HashSet<Item> _extraIndentedItems = new();
        private Position _position = new();

        public PrettyPrint(Checklist checklist, PrettyPrintOptions option = Full) {
            _option = option;
            checklist.Accept(this);
        }

        public void PreVisit(Checklist checklist, Person creator) {
            _result += String.Format("{0}Checklist created by {1}\n", Indention, creator);
            _indentionLevel++;
        }

        public void PostVisit(Checklist checklist, Person creator) {
            _indentionLevel--;
        }

        public void Visit(
            BooleanItem item,
            Guid id,
            string question,
            bool? value,
            Dictionary<Person, List<Operation>> operations
        ) {
            LabelIndention(item);
            _result += String.Format("{0}Question: [{1}] {2} Value: {3}\n", Indention, _position, question, Format(value));
            OperationsDescription(operations);
            LabelUndention(item);
            _position.Increment();
        }

        public void Visit(
            MultipleChoiceItem item,
            Guid id,
            string question,
            object? value,
            List<object> choices,
            Dictionary<Person, List<Operation>> operations
        ) {
            LabelIndention(item);
            _result += String.Format("{0}Boolean Item with value {1}\n", Indention, Format(value));
            OperationsDescription(operations);
            LabelUndention(item);
        }

        public void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {
            LabelIndention(item);
            _result += String.Format("{0}Conditional [{1}]\n", Indention, _position);
            _conditionalItems.Add(new Triple(baseItem, successItem, failureItem));
            _indentionLevel++;
            _position.Deeper();
        }

        public void PostVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {
            _indentionLevel--;
            LabelUndention(item);
            _position.Truncate();
            _position.Increment();
        }

        public void PreVisit(OrItem item, Item item1, Item item2) {
            LabelIndention(item);
            _result += String.Format("{0}Either/Or [{1}]\n", Indention, _position);
            _indentionLevel++;
            _position.Deeper();
        }

        public void PostVisit(OrItem item, Item item1, Item item2) {
            _indentionLevel--;
            LabelUndention(item);
            _position.Truncate();
            _position.Increment();
        }

        public void PreVisit(NotItem item, Item negatedItem) {
            LabelIndention(item);
            _result += String.Format("{0}Not (the following) [{1}]\n", Indention, _position);
            _indentionLevel++;
            _position.Deeper();
        }

        public void PostVisit(NotItem item, Item negatedItem) {
            _indentionLevel--;
            LabelUndention(item);
            _position.Truncate();
            _position.Increment();
        }

        public void PreVisit(GroupItem item, List<Item> childItems) {
            LabelIndention(item);
            _result += String.Format("{0}Group of Items [{1}]\n", Indention, _position);
            _position.Deeper();
            _indentionLevel++;
        }

        public void PostVisit(GroupItem item, List<Item> childItems) {
            _indentionLevel--;
            LabelUndention(item);
            _position.Truncate();
            _position.Increment();
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

            internal bool IsEmpty() => _baseItem == null && _successItem == null && _failItem == null;
        }

        public enum PrettyPrintOptions {
            Full, NoOperations
        }
    }
}