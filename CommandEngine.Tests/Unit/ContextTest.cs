﻿using CommandEngine.Commands;
using CommandEngine.Tasks;
using CommandEngine.Tests.Util;
using static CommandEngine.Tests.Unit.TestConclusion;
using static CommandEngine.Tests.Unit.TestLabels;
using static CommandEngine.Commands.SerialCommand;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandState;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tasks.CommandTask;

namespace CommandEngine.Tests.Unit
{
	public class ContextTest
	{
		[Fact]
		public void SimpleConclusion()
		{
			var context = new Context();
			Assert.Throws<MissingContextInformationException>(() => context[Conclusion]);
			context[Conclusion] = NotPay;
			Assert.Equal(NotPay, context[Conclusion]);
		}
		
		[Fact]
		public void TaskWithConclusion()
		{
			var command = Sequence(
					AlwaysSuccessful.Otherwise(AlwaysSuccessful),
					new ConclusionTask(NotPay).Otherwise(AlwaysSuccessful),
					AlwaysSuccessful.Otherwise(AlwaysSuccessful)
			);
			var context = new Context();
			var e = Assert.Throws<ConclusionException>(() => command.Execute(context));
			Assert.Equal(NotPay, e.Conclusion);
			command.AssertStates(Executed, Executed, Initial);

		}
		
		[Fact]
		public void ExtractSubContext()
		{
			var c = new Context {
				["A"] = "A",
				["B"] = "B",
				["C"] = "C"
			};
			var sub = c.SubContext(Labels("A", "B"));
			Assert.Equal("A", sub["A"]);
			Assert.Equal("B", sub["B"]);
			Assert.Throws<MissingContextInformationException>(() => sub["C"]);
		}

		[Fact]
		public void TaskWithSubContext()
		{
			var c = Context("A", "B", "C");
			var neededLabels = Labels("A", "B");
			var changedLabels = Labels("D", "B");
			var missingLabels = Labels("C");
			var command = Sequence(
					AlwaysSuccessful.Otherwise(AlwaysSuccessful),
					new ContextTask(neededLabels, changedLabels, missingLabels).Otherwise(AlwaysSuccessful)
			);
			Assert.Throws<MissingContextInformationException>(() => c["D"]);
			Assert.Equal(Succeeded, command.Execute(c));
			Assert.Equal("DChanged", c["D"]);
			Assert.Equal("BChanged", c["B"]);
		}
        [Fact]
        public void UndoTaskWithSubContext()
        {
            var c = Context("A", "B", "C");
            var neededLabels = Labels("A", "B");
            var changedLabels = Labels("D", "B");
            var missingLabels = Labels("C");
            var command = Sequence(
                    AlwaysSuccessful.Otherwise( new ContextTask(neededLabels, changedLabels, missingLabels)),
                    AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<MissingContextInformationException>(() => c["D"]);
            Assert.Equal(Reverted, command.Execute(c));
            Assert.Equal("DChanged", c["D"]);
            Assert.Equal("BChanged", c["B"]);
        }

        [Fact]
        public void TaskWithMissingLabel()
        {
            var c = Context("A", "B", "C");
            var neededLabels = Labels("A", "B", "D");
            var changedLabels = Labels("D", "B");
            var missingLabels = Labels("C");
            var command = Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    new ContextTask(neededLabels, changedLabels, missingLabels).Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<MissingContextInformationException>(() => c["D"]);
            Assert.Equal(Succeeded, command.Execute(c));
            Assert.Equal("DChanged", c["D"]);
            Assert.Equal("BChanged", c["B"]);
        }
		
		[Fact]
        public void TaskWithRequiredLabel()
        {
            var c = Context("A", "B", "C");
            var neededLabels = Labels("A", "B", "D");
            var changedLabels = Labels("D", "B");
            var missingLabels = Labels("C");
            var command = Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    Mandatory(new ContextTask(neededLabels, changedLabels, missingLabels)).Otherwise(AlwaysSuccessful)
            );
            Assert.False(c.Has("D"));
            Assert.Throws<TaskSuspendedException>(() => command.Execute(c));
			c["D"] = "D";
            Assert.Equal(Succeeded, command.Execute(c));
            Assert.Equal("DChanged", c["D"]);
            Assert.Equal("BChanged", c["B"]);
        }

        private static List<object> Labels(params string[] labels) => [..labels];

        private static Context Context(params string[] labels)
        {
            var result = new Context();
			foreach(var label in labels) result[label] = label.ToUpper();
			return result;
        }
    }
	internal enum TestConclusion
	{
		Pay,
		NotPay,
		CallPolice
	}

	internal enum TestLabels
	{
		Conclusion
	}
}
