using CommandEngine.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CommandEngine.Tests.Unit.PermanentStatus;
using System.Threading.Tasks;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Tests.Unit
{
    public class SimpleCommandTest
    {
        [Fact]
        public void Successful()
        {
            var command = new SimpleCommand(alwaysSuccessfull, alwaysSuccessfull);
            Assert.Equal(Succeeded,command.Execute());
            Assert.Equal(Reverted,command.Undo());
        }
        [Fact]
        public void FailedTask()
        {
            Assert.Equal(Failed, new SimpleCommand(alwaysFail, alwaysFail).Execute());
        }
        [Fact]
        public void SuspendedTask()
        {
            Assert.Equal(Suspended, new SimpleCommand(alwaysSuspended, alwaysSuspended).Execute());
        }
        [Fact]
        public void TaskCrashed()
        {
            Assert.Equal(Failed, new SimpleCommand(new CrashingTask(), alwaysSuspended).Execute());
        }
        [Fact]
        public void UndoFails()
        {   var command = new SimpleCommand(alwaysSuccessfull, alwaysFail);
            Assert.Equal(Succeeded, command.Execute());
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo());
        }
        
        [Fact]
        public void UndoCrashes()
        {   var command = new SimpleCommand(alwaysSuccessfull, new CrashingTask());
            Assert.Equal(Succeeded, command.Execute());
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo());
        }

    }
    internal class PermanentStatus(CommandStatus status) : CommandTask
    {
        internal readonly static PermanentStatus alwaysSuccessfull = new(Succeeded);
        internal readonly static PermanentStatus alwaysFail = new(Failed);
        internal readonly static PermanentStatus alwaysSuspended = new(Suspended);

        public CommandStatus Execute() => status;

    }
    internal class CrashingTask : CommandTask
    {
        public CommandStatus Execute()
        {
            throw new InvalidOperationException("unable to execute this task");
        }
    }
}
