/*
 * Copyright (c) 2025 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using Engine.Persons;

namespace Engine.Items;

// Supports walking a Checklist Item hierarchy
public interface ChecklistVisitor {
    void PreVisit(Checklist checklist, Person creator) {}
    void PostVisit(Checklist checklist, Person creator) {}
    void Visit(BooleanItem item, Dictionary<Person, List<Operation>> operations) {}
    void Visit(MultipleChoiceItem item, Dictionary<Person, List<Operation>> operations) {}
    void PreVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {}
    void PostVisit(ConditionalItem item, Item baseItem, Item? successItem, Item? failureItem) {}
    void PreVisit(NotItem item, Item negatedItem) {}
    void PostVisit(NotItem item, Item negatedItem) {}
    void PreVisit(OrItem item, Item item1, Item item2) {}
    void PostVisit(OrItem item, Item item1, Item item2) {}
}