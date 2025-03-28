using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Persons;
using static System.Text.Json.JsonSerializer;

namespace Engine.Items
{
    internal class ChecklistDeserializer
    {
        internal Checklist Result => new Checklist(new Person(0,0),"".TrueFalse());
        internal ChecklistDeserializer(string memento)
        {
            try
            {
                var dto = Deserialize<List<ItemDto>>(memento) ?? throw new InvalidOperationException("Invalid Json");
                
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
    }
}
