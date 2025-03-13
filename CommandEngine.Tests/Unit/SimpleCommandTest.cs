using CommandEngine.Commands;
using CommandEngine.Tests.Util;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandStatus;
using CommandEngine.Tasks;

namespace CommandEngine.Tests.Unit
{
    public class SimpleCommandTest
    {
        [Fact]
        public void Successful()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            Assert.Equal(Succeeded,command.Execute(new Context()));
            Assert.Equal(Reverted,command.Undo(new Context()));
        }
        
        [Fact]
        public void FailedTask()
        {
            Assert.Equal(Failed, new SimpleCommand(AlwaysFail, AlwaysFail).Execute(new Context()));
        }
        
        [Fact]
        public void SuspendedTask()
        {
            Assert.Throws<TaskSuspendedException>(() => new SimpleCommand(AlwaysSuspended, AlwaysSuspended).Execute(new Context()));
        }
        
        [Fact]
        public void TaskCrashed()
        {
            Assert.Equal(Failed, new SimpleCommand(new CrashingTask(), AlwaysSuspended).Execute(new Context()));
        }
        
        [Fact]
        public void UndoFails()
        {   var command = new SimpleCommand(AlwaysSuccessful, AlwaysFail);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo(new Context()));
        }
        
        [Fact]
        public void UndoCrashes()
        {   var command = new SimpleCommand(AlwaysSuccessful, new CrashingTask());
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo(new Context()));
        }

        [Fact]
        public void UndoSuspends()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuspended);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Throws<TaskSuspendedException>(() => command.Undo(new Context()));
        }

        [Fact]
        public void ExecuteTwice()
        {
            var command = new SimpleCommand(new RunOnceTask(), AlwaysSuccessful);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Equal(Succeeded, command.Execute(new Context()));
        }

        [Fact]
        public void UndoTwice()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Equal(Reverted, command.Undo(new Context()));
            Assert.Throws<InvalidOperationException>(()=> command.Undo(new Context()));
            Assert.Throws<InvalidOperationException>(() => command.Execute(new Context()));
        }

        [Fact]
        public void UndoBeforeExecute()
        {
            var command = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            Assert.Throws<InvalidOperationException>(() => command.Undo(new Context()));
        }
    }
}
