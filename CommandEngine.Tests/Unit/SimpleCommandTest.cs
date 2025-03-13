using CommandEngine.Commands;
using CommandEngine.Tests.Util;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Tests.Unit
{
    public class SimpleCommandTest
    {
        [Fact]
        public void Successful()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            Assert.Equal(Succeeded,command.Execute());
            Assert.Equal(Reverted,command.Undo());
        }
        
        [Fact]
        public void FailedTask()
        {
            Assert.Equal(Failed, new SimpleCommand(AlwaysFail, AlwaysFail).Execute());
        }
        
        [Fact]
        public void SuspendedTask()
        {
            Assert.Throws<TaskSuspendedException>(() => new SimpleCommand(AlwaysSuspended, AlwaysSuspended).Execute());
        }
        
        [Fact]
        public void TaskCrashed()
        {
            Assert.Equal(Failed, new SimpleCommand(new CrashingTask(), AlwaysSuspended).Execute());
        }
        
        [Fact]
        public void UndoFails()
        {   var command = new SimpleCommand(AlwaysSuccessful, AlwaysFail);
            Assert.Equal(Succeeded, command.Execute());
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo());
        }
        
        [Fact]
        public void UndoCrashes()
        {   var command = new SimpleCommand(AlwaysSuccessful, new CrashingTask());
            Assert.Equal(Succeeded, command.Execute());
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo());
        }

        [Fact]
        public void UndoSuspends()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuspended);
            Assert.Equal(Succeeded, command.Execute());
            Assert.Throws<TaskSuspendedException>(() => command.Undo());
        }

        [Fact]
        public void ExecuteTwice()
        {
            var command = new SimpleCommand(new RunOnceTask(), AlwaysSuccessful);
            Assert.Equal(Succeeded, command.Execute());
            Assert.Equal(Succeeded, command.Execute());
        }

        [Fact]
        public void UndoTwice()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            Assert.Equal(Succeeded, command.Execute());
            Assert.Equal(Reverted, command.Undo());
            Assert.Throws<InvalidOperationException>(()=> command.Undo());
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void UndoBeforeExecute()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            Assert.Throws<InvalidOperationException>(() => command.Undo());
        }
    }
}
