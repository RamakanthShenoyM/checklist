using CommandEngine.Commands;

namespace CommandEngine.Tests.Util
{
    internal static class TestExtensions
    {
        internal static void AssertStates(this Command command, params CommandState[] expectedStates)=>
        Assert.Equal(expectedStates.ToList(), new StateVisitor(command).States);
        
        

        internal static List<Enum> Labels(params Enum[] labels) => [.. labels];
    }
}
