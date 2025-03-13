using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandEngine.Commands;

namespace CommandEngine.Tests.Util
{
    internal static class TestExtentions
    {
        internal static SimpleCommand Otherwise(this CommandTask executeTask,CommandTask revertTask)=>
           new SimpleCommand(executeTask,revertTask);

        internal static void AssertStates(this Command command, params CommandState[] expectedStates)=>
        Assert.Equal(expectedStates.ToList(), new StateVisitor(command).States);

        internal static Command ToCommand(this Command[] commands) => new SerialCommand(commands.ToList());

    }
}
