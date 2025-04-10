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
                                          object? Value=null,
                                          ChecklistRequestValueType? ValueType=null,
                                          PersonDto? PersonToModify = null,
                                          ChecklistRole? Role=null);

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
                    case ChecklistRequestType.Reset:
                        ExecuteReset(requestDto, checklist);
                        break;
                    case ChecklistRequestType.AddPerson:
                        ExecuteAddPerson(requestDto,checklist);
                        break;
                    case ChecklistRequestType.RemovePerson:
                        ExecuteRemovePerson(requestDto, checklist);
                        break;
                } ;
            }
        }

        private static void ExecuteSet(ChecklistRequestDto requestDto,Checklist checklist)
        {
            var item = (SimpleItem)checklist.P(requestDto.ItemPosition.ToPosition());
            var person = new Person(requestDto.Person.OrganizationId, requestDto.Person.PersonId);
            var value = Value(requestDto.Value, requestDto.ValueType);
            person.Sets(item).To(value);
        }

        private static void ExecuteReset(ChecklistRequestDto requestDto, Checklist checklist)
        {
            var item = (SimpleItem)checklist.P(requestDto.ItemPosition.ToPosition());
            var person = new Person(requestDto.Person.OrganizationId, requestDto.Person.PersonId);
            person.Reset(item);
        }

        private static void ExecuteAddPerson(ChecklistRequestDto requestDto, Checklist checklist)
        {
            var item = checklist.P(requestDto.ItemPosition.ToPosition());
            var person = new Person(requestDto.Person.OrganizationId, requestDto.Person.PersonId);
            var personToAdd = new Person(requestDto.PersonToModify.OrganizationId, requestDto.PersonToModify.PersonId);
            var role = requestDto.Role switch
            {
                ChecklistRole.Creator =>Role.Creator,
                ChecklistRole.Owner=>Role.Owner,
                ChecklistRole.Viewer=>Role.Viewer,
                _ => throw new ArgumentOutOfRangeException(nameof(requestDto.Role), requestDto.Role, null)
            };
            person.Add(personToAdd).As(role).To(item);
        }

        private static void ExecuteRemovePerson(ChecklistRequestDto requestDto, Checklist checklist)
        {
            var item = checklist.P(requestDto.ItemPosition.ToPosition());
            var person = new Person(requestDto.Person.OrganizationId, requestDto.Person.PersonId);
            var personToRemove = new Person(requestDto.PersonToModify.OrganizationId, requestDto.PersonToModify.PersonId);
            person.Remove(personToRemove).From(item);
        }

        private static object Value(object requestDtoValue, ChecklistRequestValueType? requestDtoValueType)
        {
            return requestDtoValueType switch
            {
                ChecklistRequestValueType.BooleanValue => BooleanParser(requestDtoValue),
                ChecklistRequestValueType.IntegerValue=>IntegerParser(requestDtoValue),
                ChecklistRequestValueType.DoubleValue => DoubleParser(requestDtoValue),
                ChecklistRequestValueType.StringValue => StringParser(requestDtoValue),
                ChecklistRequestValueType.CharacterValue => CharacterParser(requestDtoValue),
                _ => throw new ArgumentOutOfRangeException(nameof(requestDtoValueType), requestDtoValueType, null)
            };
        }

        private static bool BooleanParser(object requestDtoValue)
        {
            if (requestDtoValue is bool value) return value;
            return bool.Parse(requestDtoValue.ToString());
        }

        private static int IntegerParser(object requestDtoValue)
        {
            if (requestDtoValue is int value) return value;
            return int.Parse(requestDtoValue.ToString());
        }

        private static string StringParser(object requestDtoValue)
        {
            if (requestDtoValue is string value) return value;
            return requestDtoValue.ToString();
        }

        private static double DoubleParser(object requestDtoValue)
        {
            if (requestDtoValue is double value) return value;
            return double.Parse(requestDtoValue.ToString());
        }

        private static char CharacterParser(object requestDtoValue)
        {
            if (requestDtoValue is char value) return value;
            return requestDtoValue.ToString()[0];
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

    public enum ChecklistRole
    {
        Creator,
        Owner,
        Viewer
    }
}
