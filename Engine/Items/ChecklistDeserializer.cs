﻿using Engine.Persons;
using static System.Text.Json.JsonSerializer;
using static Engine.Items.NullItem;

namespace Engine.Items
{
    internal class ChecklistDeserializer
    {
        private int level;
        private readonly Stack<List<Item>> items = new();
        private readonly Person person;
        internal Checklist Result => new(person, items.Peek()[0]);

        internal ChecklistDeserializer(string memento)
        {
            var (personDto, dtos) = Deserialize<CheckListDto>(memento) ?? throw new InvalidOperationException("Invalid Json");
            person = new Person(personDto.OrganizationId, personDto.PersonId);
            level = Level(dtos[0]);
            for (var i = 0; i < level; i++) items.Push([]);
            foreach (var dto in dtos)
            {
                var subItems = AdjustStack(dto);
                items.Peek().Add(dto.ItemClassName switch
                {
                    "BooleanItem" => TrueFalse(dto),
                    "GroupItem" => new GroupItem(subItems),
                    "MultipleChoiceItem" => MultipleChoice(dto),
                    "ConditionalItem" => new ConditionalItem(subItems[0], subItems[1], subItems[2]),
                    "OrItem" => new OrItem(subItems[0], subItems[1]),
                    "NotItem" => new NotItem(subItems[0]),
                    "NullItem" => Instance,
                    _ => throw new InvalidOperationException($"Unknown item class name {dto.ItemClassName}")
                });
            }


        }


        private BooleanItem TrueFalse(ItemDto dto)
        {
            var result = new BooleanItem(dto.Question ?? throw new InvalidOperationException("Improper DTO for BooleanItem: No question specified"), dto.Id);
            foreach (var operation in dto.Operations ?? [])
            {
                result.AddOperation(new Person(operation.Person.OrganizationId, operation.Person.PersonId), operation.Operations);
            }
            var value = dto.Value?.ValueValue ?? throw new InvalidOperationException("Improper DTO for BooleanItem: No value specified");
            if (value != "") result.Be(bool.Parse(value));
            return result;
        }

        private MultipleChoiceItem MultipleChoice(ItemDto dto)
        {
            if (dto.Choices == null || dto.Choices.Count == 0) throw new InvalidOperationException("Improper DTO for MultipleChoiceItem: No choices specified");
            var choices = dto.Choices.Select(c => Value(c.ValueClass ?? throw new InvalidOperationException("Improper DTO for MultipleChoiceItem: No value class specified"), c.ValueValue)).ToList();
            var result = new MultipleChoiceItem(dto.Question ?? throw new InvalidOperationException("Improper DTO for MultipleChoiceItem: No question specified"), choices[0], dto.Id, choices.Skip(1).ToArray());
            var value = dto.Value?.ValueValue ?? throw new InvalidOperationException("Improper DTO for MultipleChoiceItem: No value specified");
            if (value != "") result.Be(Value(dto.Value?.ValueClass ?? throw new InvalidOperationException("Improper DTO for MultipleChoiceItem: No value class specified"), value));
            return result;
        }

        private static object Value(string entryValueType, string entryValueValue)
        {
            Type valueType = FoundType(entryValueType);
            if (valueType.IsEnum)
                return Enum.Parse(valueType, entryValueValue);

            return Convert.ChangeType(entryValueValue, valueType);
        }

        internal static Type FoundType(string fullTypeName)
        {
            // Check all currently loaded assemblies in the AppDomain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null) return type;
            }

            throw new InvalidOperationException($"Type {fullTypeName} not found");
        }

        private GroupItem Group(ItemDto dto, List<Item> items)
        {
            return new GroupItem(items);
        }

        private List<Item> AdjustStack(ItemDto dto)
        {
            var newLevel = Level(dto);
            var diff = newLevel - level;
            if (diff < -1) throw new InvalidOperationException($"Invalid level {newLevel} from {level}");
            if (diff == 0) return [];
            level = newLevel;
            if (diff == -1) return items.Pop();
            for (int i = 0; i < diff; i++) items.Push([]);
            return [];
        }

        private static int Level(ItemDto dto)
        {
            return dto.Position.Count(p => p == '.') + 1;
        }
    }

}