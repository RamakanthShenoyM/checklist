using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandEngine.Commands;
using CommandEngine.Tasks;
using static CommandEngine.Tests.Util.PermanentStatus;

namespace CommandEngine.Tests.Unit
{
    public class JsonTest
    {
        [Fact]
        public void TwoSimpleCommands()
        {
            var originalCommand = "Top Level".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful), 
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
                );

            var json = originalCommand.ToJson();
            var restoredCommand = json.FromJson();
            Assert.Equal(originalCommand, restoredCommand);
        }
    }
}
