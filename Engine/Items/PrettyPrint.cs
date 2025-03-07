/*
 * Copyright (c) 2025 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using Engine.Persons;

namespace Engine.Items;

// Understands SOMETHING_IDIOT
public class PrettyPrint : ChecklistVisitor {
    private int _indentionLevel = 0;

    private String result = "";

    public PrettyPrint(Checklist checklist) {
        checklist.Accept(this);
    }

    public void PreVisit(Checklist checklist, Person creator) {
        result += String.Format("{0}Checklist created by {1}\n", Indention, creator);
        _indentionLevel++;
    }

    public void PostVisit(Checklist checklist, Person creator) {
        _indentionLevel--;
    }

    public void Visit(BooleanItem item, bool? value, Dictionary<Person, List<Operation>> operations) {
        result += String.Format("{0}Boolean Item with value {1}\n", Indention, Format(value));
    }

    private String Indention { get { return new String(' ', _indentionLevel * 2); } }

    private String Format(object? value) => value is null ? "<unknown>" : value.ToString();

    public string Result() => result;
}