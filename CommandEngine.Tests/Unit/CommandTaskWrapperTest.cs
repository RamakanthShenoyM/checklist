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

namespace CommandEngine.Tests.Unit
{
	public class CommandTaskWrapperTest
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
			var wrapper = new CommandTaskWrapper(template);
			var wrapperClone = wrapper.Clone();
			Assert.Equal(Succeeded, wrapperClone.Execute(c));

		}
	}
}
