/*
 * Copyright (c) 2025 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using System.Collections.Specialized;
using Engine.Persons;

namespace Engine.Items;

// Understands SOMETHING_IDIOT
public class PrettyPrint : ChecklistVisitor {
    private delegate void ItemWriter();

    private int _indentionLevel;
    private readonly List<Triple> _conditionalItems = [];
    private String _result = "";

    public PrettyPrint(Checklist checklist) {
        checklist.Accept(this);
    }

    public void PreVisit(Checklist checklist, Person creator) {
        _result += String.Format("{0}Checklist created by {1}\n", Indention, creator);
        _indentionLevel++;
    }

    public void PostVisit(Checklist checklist, Person creator) {
        _indentionLevel--;
    }

    public void Visit(BooleanItem item, bool? value, Dictionary<Person, List<Operation>> operations) {
        HandleConditional(item,
            () => {
                _result += String.Format("{0}Boolean Item with value {1}\n", Indention, Format(value));
                OperationsDescription(operations);
            });
    }

    public void Visit(MultipleChoiceItem item, object? value, Dictionary<Person, List<Operation>> operations) {
        _result += String.Format("{0}Boolean Item with value {1}\n", Indention, Format(value));
        OperationsDescription(operations);
    }

    public void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {
        _result += String.Format("{0}Conditional\n", Indention);
        _conditionalItems.Insert(0, new Triple(baseItem, successItem, failureItem));
        _indentionLevel++;
    }

    public void PostVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {
        _indentionLevel--;
    }

    private void OperationsDescription(Dictionary<Person, List<Operation>> operations) {
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

    private String Indention {
        get { return new String(' ', _indentionLevel * 2); }
    }

    private String Format(object? value) => value?.ToString() ?? "<unknown>";

    private String Operations(List<Operation> operations) =>
        String.Join(",", operations.Select(o => Format(o)).ToArray());

    private void HandleConditional(Item item, ItemWriter writer) {
        if (_conditionalItems.Count > 0) {
            var label = _conditionalItems[0].Matches(item);
            if (label.Length > 0) {
                _result += String.Format("{0}{1}\n", Indention, label);
                _indentionLevel++;
                writer.Invoke();
                removeTripleIfEmpty();
                _indentionLevel--;
                return;
            }
        }
        writer.Invoke();
    }

    private void removeTripleIfEmpty() {
        if (_conditionalItems[0].IsEmpty()) _conditionalItems.RemoveAt(0);
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
}