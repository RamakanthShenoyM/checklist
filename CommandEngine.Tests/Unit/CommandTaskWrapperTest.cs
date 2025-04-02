using CommandEngine.Commands;
using CommandEngine.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Unit.TestLabels;
using static CommandEngine.Tests.Util.TestExtensions;
using CommandEngine.Tests.Util;
using Xunit.Sdk;
using Xunit.Abstractions;
using static CommandEngine.Tests.Util.SuspendLabels;
using static CommandEngine.Commands.CommandEventType;

namespace CommandEngine.Tests.Unit
{
	public class CommandTaskWrapperTest(ITestOutputHelper testOutput)
	{
		[Fact]
		public void WrapSimpleCommand()
		{
			var c = new Context();
			c[A] = "A";
			c[B] = "B";
			c[C] = "C";
			var template = "Incident process one".Template("Sub Command".Sequence(
				AlwaysSuccessful.NoReverting()
				));
			var wrapper = new CommandTaskWrapper(template, [], []);
			var wrapperClone = wrapper.Clone();
			Assert.Equal(Succeeded, wrapperClone.Execute(c));

		}

		[Fact]
		public void WrapWithContext()
		{
			var c = Context(A, B, C, G);
			var neededLabels = Labels(A, B);
			var changedLabels = Labels(D, B);
			var missingLabels = Labels(C);
			var child = "Child Environment".Template("Sub Command".Sequence(
				new ContextTask(neededLabels, changedLabels, missingLabels).NoReverting()
				));
			var wrapper = new CommandTaskWrapper(child, neededLabels, changedLabels);
			var master = "Master Environment".Template("Primary Group".Sequence(
				wrapper.Otherwise(AlwaysSuccessful)
			));

			var originalEnvironment = CommandEnvironment.FreshEnvironment(master,c);
			Assert.Equal(Succeeded, originalEnvironment.Execute());
			Assert.Equal("DChanged", c[D]);
			testOutput.WriteLine(c.History.ToString());
			Assert.Equal(4, c.History.Events(ValueChanged).Count);
		}

		[Fact]
		public void WrapWithSuspension()
		{
			var c = Context(A, B, C, G);
			var neededLabels = Labels(A, B);
			var changedLabels = Labels(D, B);
			var missingLabels = Labels(C);
			var child = "Child Environment".Template("Sub Command".Sequence(
				new ContextTask(neededLabels, changedLabels, missingLabels).NoReverting(),
				new SuspendFirstOnly().NoReverting()
				));
			var wrapperNeeds = new List<Enum>(neededLabels) { HasRun };
			var wrapperChanged = new List<Enum>(changedLabels) { HasRun };
			var wrapper = new CommandTaskWrapper(child, wrapperNeeds, wrapperChanged);
			var master = "Master Environment".Template("Primary Group".Sequence(
				wrapper.Otherwise(AlwaysSuccessful)
			));
			var originalEnvironment = CommandEnvironment.FreshEnvironment(master, c);
			Assert.Throws<TaskSuspendedException>(() => originalEnvironment.Execute());
            Assert.Equal("DChanged", c[D]);
            var memento = originalEnvironment.ToMemento();
            var restoredEnvironment = CommandEnvironment.FromMemento(memento);
			Assert.Equal(Succeeded, restoredEnvironment.Execute());
            testOutput.WriteLine(restoredEnvironment.History().ToString());
        }
	}
}
