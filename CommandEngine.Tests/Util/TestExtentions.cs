using CommandEngine.Commands;
using CommandEngine.Tasks;

namespace CommandEngine.Tests.Util
{
    internal static class TestExtentions
    {
        internal static SimpleCommand Otherwise(this CommandTask executeTask,CommandTask revertTask)=>
           new SimpleCommand(executeTask,revertTask);

        internal static void AssertStates(this Command command, params CommandState[] expectedStates)=>
        Assert.Equal(expectedStates.ToList(), new StateVisitor(command).States);
    }
}
