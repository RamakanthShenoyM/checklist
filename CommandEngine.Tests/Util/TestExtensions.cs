using CommandEngine.Commands;
using CommandEngine.Tasks;

namespace CommandEngine.Tests.Util
{
    internal static class TestExtensions
    {
        internal static void AssertStates(this Command command, params CommandState[] expectedStates)=>
        Assert.Equal(expectedStates.ToList(), new StateVisitor(command).States);
        
        

        internal static List<Enum> Labels(params Enum[] labels) => [.. labels];
		internal static Context Context(params Enum[] labels)
		{
			var result = new Context();
			foreach (var label in labels) result[label] = label.ToString().ToUpper();
			return result;
		}

        internal static History History(this CommandEnvironment environment) => new HistoryDump(environment).Result;

        private class HistoryDump : CommandVisitor
        {
            private History? _history;
            internal History Result => _history ?? throw new InvalidOperationException("Visit Failure");
            internal HistoryDump(CommandEnvironment environment)
            {
                environment.Accept(this);
            }

            public void Visit(History history, List<string> events) => _history = history;
        }
    }
}
