/*
 * Copyright (c) 2025 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using Engine.Persons;

namespace Engine.Items;

// Understands SOMETHING_IDIOT
public class PrettyPrint : ChecklistVisitor {
    private int _indentionLevel;

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
        _result += String.Format("{0}Boolean Item with value {1}\n", Indention, Format(value));
        OperationsDescription(operations);
    }

    public void Visit(MultipleChoiceItem item, object? value, Dictionary<Person, List<Operation>> operations) {
        _result += String.Format("{0}Boolean Item with value {1}\n", Indention, Format(value));
        OperationsDescription(operations);
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

    private String Indention { get { return new String(' ', _indentionLevel * 2); } }

    private String Format(object? value) => value?.ToString() ?? "<unknown>";

    private String Operations(List<Operation> operations) => 
        String.Join(",", operations.Select(o => Format(o)).ToArray());

    public string Result() => _result;
}