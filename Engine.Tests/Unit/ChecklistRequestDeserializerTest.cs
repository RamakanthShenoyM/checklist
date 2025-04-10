using System;
using System.Text.Json;
using Engine.Items;
using Engine.Persons;
using Xunit;
using static Engine.Items.ChecklistRequestDeserializer;
using static Engine.Items.ChecklistRequestType;
using static Engine.Items.ChecklistRequestValueType;
using static Engine.Persons.Operation;
using static Engine.Tests.Unit.CarpetColor;

namespace Engine.Tests.Unit
{
    public class ChecklistRequestDeserializerTest
    {
        private static readonly Person Creator = new Person(100, 200);

        [Fact]
        public void DeserializeSetCommand()
        {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse(),
                "Multiple choice question1".Choices("A", "B","C"),
                "Multiple choice question2".Choices(1,2,3),
                "Multiple choice question3".Choices(1.0, 2.0, 3.0),
                "Multiple choice question4".Choices('A', 'B', 'C'),
                "Which Carpet Color?".Choices(RedCarpet, GreenCarpet, NoCarpet)
            );
            var setDto1 = new ChecklistRequestDto(new PersonDto(100,200), ChecklistRequestType.Set, ItemPosition:"P[0.0]",Value: "true", ValueType:BooleanValue);
            var setDto2 = new ChecklistRequestDto(new PersonDto(100,200), ChecklistRequestType.Set, ItemPosition: "P[0.1]", Value: "A", ValueType: StringValue);
            var setDto3 = new ChecklistRequestDto(new PersonDto(100,200), ChecklistRequestType.Set, ItemPosition: "P[0.2]", Value: "1", ValueType: IntegerValue);
            var setDto4 = new ChecklistRequestDto(new PersonDto(100,200), ChecklistRequestType.Set, ItemPosition: "P[0.3]", Value: "1.0", ValueType: DoubleValue);
            var setDto5 = new ChecklistRequestDto(new PersonDto(100,200), ChecklistRequestType.Set, ItemPosition: "P[0.4]", Value: "A", ValueType: CharacterValue); 
            var setDto6 = new ChecklistRequestDto(new PersonDto(100,200), ChecklistRequestType.Set, ItemPosition: "P[0.5]", Value: "GreenCarpet", ValueType: EnumValue);
            var inputDto = new ChecklistApiDto(Guid.NewGuid(), [setDto1,setDto2,setDto3,setDto4,setDto5,setDto6]);
            var json = JsonSerializer.Serialize(inputDto);
            new ChecklistRequestDeserializer(json).Execute(checklist);

            Assert.Equal(ChecklistStatus.Succeeded,checklist.Status());
        }

        [Fact]
        public void DeserializeResetCommand()
        {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var item = (SimpleItem)checklist.P(0);
            Creator.Sets(item).To(true);

            var resetDto = new ChecklistRequestDto(new PersonDto(100, 200), Reset, ItemPosition: "P[0]");
            var inputDto = new ChecklistApiDto(Guid.NewGuid(), [resetDto]);
            var json = JsonSerializer.Serialize(inputDto);
            new ChecklistRequestDeserializer(json).Execute(checklist);

            Assert.Equal(ChecklistStatus.InProgress, checklist.Status());
        }

        [Fact]
        public void DeserializeAddPersonCommand()
        {
            var checklist= Creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var item = (SimpleItem)checklist.P(0);

            var addPersonDto = new ChecklistRequestDto(new PersonDto(100, 200), AddPerson, ItemPosition: "P[0]", PersonToModify: new PersonDto(100, 300), Role: ChecklistRole.Owner);
            var inputDto = new ChecklistApiDto(Guid.NewGuid(), [addPersonDto]);
            var json = JsonSerializer.Serialize(inputDto);
            new ChecklistRequestDeserializer(json).Execute(checklist);

            var Owner = new Person(100, 300);
            Assert.True(Owner.Can(Operation.Set).On(item));
            Assert.False(Owner.Can(ModifyChecklist).On(item));
        }

        [Fact]
        public void DeserializeRemovePersonCommand()
        {
            var checklist = Creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var item = (SimpleItem)checklist.P(0);

            var owner = new Person(100, 300);
            Creator.Add(owner).As(Role.Owner).To(item);
            Assert.True(owner.Can(Operation.Set).On(item));
            Assert.False(owner.Can(ModifyChecklist).On(item));

            var removePersonDto = new ChecklistRequestDto(new PersonDto(100, 200), RemovePerson, ItemPosition: "P[0]", PersonToModify: new PersonDto(100, 300));
            var inputDto = new ChecklistApiDto(Guid.NewGuid(), [removePersonDto]);
            var json = JsonSerializer.Serialize(inputDto);
            new ChecklistRequestDeserializer(json).Execute(checklist);

            Assert.False(owner.Can(Operation.Set).On(item));
        }
    }
}
