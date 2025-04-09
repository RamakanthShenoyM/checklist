using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Engine.Items;
using Engine.Persons;
using Xunit;
using static Engine.Items.ChecklistRequestDeserializer;
using static Engine.Items.ChecklistRequestType;
using static Engine.Tests.Unit.CarpetColor;

namespace Engine.Tests.Unit
{
    public class ChecklistRequestDeserializerTest
    {
        private static Person _creator = new Person(100, 200);

        [Fact]
        public void DeserializeSetCommand()
        {
            var checklist = _creator.Checklist(
                "Is US citizen?".TrueFalse()
            );
            var setDto = new ChecklistRequestDto(new PersonDto(100,200),Set,ItemPosition:"P[0]",Value: "true", ValueType:ChecklistRequestValueType.BooleanValue);
            var inputDto = new ChecklistApiDto(Guid.NewGuid(), [setDto]);
            var json = JsonSerializer.Serialize(inputDto);
            new ChecklistRequestDeserializer(json).Execute(checklist);
            Assert.Equal(ChecklistStatus.Succeeded,checklist.Status());
        }
    }
}
