using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Engine.Persons;

namespace Engine.Items
{
    public class ChecklistRequestDeserializer
    {   private readonly ChecklistApiDto _apiDto;

        public record ChecklistApiDto(Guid ChecklistId,
            List<ChecklistRequestDto> Requests);
        public record ChecklistRequestDto(PersonDto Person,
            ChecklistRequestType Type,
            string ItemPosition,
            object Value,
            ChecklistRequestValueType ValueType);

        public ChecklistRequestDeserializer(string json)
        {
            _apiDto=JsonSerializer.Deserialize<ChecklistApiDto>(json);
        }

        public void Execute(Checklist checklist)
        {
            foreach (var requestDto in _apiDto.Requests)
            {
                 switch(requestDto.Type)
                {
                    case ChecklistRequestType.Set:
                        ExecuteSet(requestDto,checklist);
                        break;



                }
                ;
            }
        }

        private void ExecuteSet(ChecklistRequestDto requestDto,Checklist checklist)
        {
            var item = (SimpleItem)checklist.P(requestDto.ItemPosition.ToPosition());
            var person = new Person(requestDto.Person.OrganizationId, requestDto.Person.PersonId);
            var value = Value(requestDto.Value, requestDto.ValueType);
            person.Sets(item).To(value);

        }

        private object Value(object requestDtoValue, ChecklistRequestValueType requestDtoValueType)
        {
            return requestDtoValueType switch
            {
                ChecklistRequestValueType.BooleanValue => BooleanParser(requestDtoValue)
            };
        }

        private bool BooleanParser(object requestDtoValue)
        {
            if (requestDtoValue is bool value) return value;
            return bool.Parse(requestDtoValue.ToString());
        }
        
    }

    public enum ChecklistRequestType
    {
        Set,
        Reset,
        AddPerson,
        RemovePerson,
        ReplaceItem,
        InsertItemAfter,
        InsertItemBefore,
        Abort
    }

    public enum ChecklistRequestValueType
    {
        BooleanValue,
        IntegerValue,
        DoubleValue,
        DateValue,
        StringValue,
        CharacterValue
    }
}
