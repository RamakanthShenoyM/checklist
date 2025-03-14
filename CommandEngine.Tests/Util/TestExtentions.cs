using CommandEngine.Commands;
using CommandEngine.Tasks;

namespace CommandEngine.Tests.Util
{
    internal static class TestExtentions
    {
        internal static void AssertStates(this Command command, params CommandState[] expectedStates)=>
        Assert.Equal(expectedStates.ToList(), new StateVisitor(command).States);
    }
}
