using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Engine.Persons;
using static System.Text.Json.JsonSerializer;

namespace Engine.Items
{
    internal class ChecklistDeserializer
    {
        private int level;
        private readonly Stack<List<Item>> items = new();
        internal Checklist Result => new Checklist(new Person(0, 0), items.Peek()[0]);
        internal ChecklistDeserializer(string memento)
        {
            try
            {
                var dtos = Deserialize<List<ItemDto>>(memento) ?? throw new InvalidOperationException("Invalid Json");
                level = Level(dtos[0]); 
                for (int i = 0; i < level; i++) items.Push([]);
                foreach (var dto in dtos)
                {
                    var subItems=AdjustStack(dto);
                    items.Peek().Add(dto.ItemClassName switch
                    {
                        "BooleanItem" => TrueFalse(dto),
                        "GroupItem"=> new GroupItem(subItems),
                        _ => throw new InvalidOperationException($"Unknown item class name {dto.ItemClassName}")
                    });

                }

            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error deserializing environment: {e.Message}", e);
            }
        }

        private BooleanItem TrueFalse(ItemDto dto)
        {
            var result = new BooleanItem(dto.Question);
            if(dto.Value.ValueValue!= "")
                result.Be(bool.Parse(dto.Value.ValueValue));
            return result;
        }

        private GroupItem Group(ItemDto dto,List<Item> items)
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
            return dto.Position.Count(p => p == '.')+1;
        }
    }
}
