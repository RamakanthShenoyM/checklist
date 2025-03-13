using CommandEngine.Tasks;
using static CommandEngine.Tests.Unit.TestConclusion;
using static CommandEngine.Tests.Unit.TestLabels;
using static CommandEngine.Commands.SerialCommand;
using static CommandEngine.Tests.Util.PermanentStatus;
using CommandEngine.Tests.Util;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Commands.CommandState;

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
			var c = new Context();
			c["A"] = "A";
			c["B"] = "B";
			c["C"] = "C";
			var sub = c.SubContext("A", "B");
			Assert.Equal("A", sub["A"]);
			Assert.Equal("B", sub["B"]);
			Assert.Throws<MissingContextInformationException>(() => sub["C"]);
		}

		[Fact]
		public void ExtractSubContextWithInvalid()
		{
			var c = new Context();
			c["A"] = "A";
			c["B"] = "B";
			var e = Assert.Throws<MissingContextInformationException>(() => c.SubContext("A", "B", "D"));
			Assert.Equal("D", e.MissingLabel);
		}

		[Fact]
		public void TaskWithSubContextConclusion()
		{
			var command = Sequence(
					AlwaysSuccessful.Otherwise(AlwaysSuccessful),
					new ConclusionTask(NotPay).Otherwise(AlwaysSuccessful),
					AlwaysSuccessful.Otherwise(AlwaysSuccessful)
			);
			var c = new Context();
			c["A"] = "A";
			var e = Assert.Throws<ConclusionException>(() => command.Execute(c.SubContext("A")));
			Assert.Equal(NotPay, e.Conclusion);
			command.AssertStates(Executed, Executed, Initial);

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
