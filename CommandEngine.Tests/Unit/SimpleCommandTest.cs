using CommandEngine.Commands;
using CommandEngine.Tests.Util;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandStatus;
using CommandEngine.Tasks;
using System;
using static CommandEngine.Commands.CommandEventType;

namespace CommandEngine.Tests.Unit
{
    public class SimpleCommandTest
    {
        [Fact]
        public void Successful()
        {
            var command = AlwaysSuccessful.Otherwise(AlwaysSuccessful);
            Assert.Equal(Succeeded,command.Execute(new Context()));
            Assert.Equal(Reverted,command.Undo(new Context()));
        }
        
        [Fact]
        public void FailedTask()
        {
            Assert.Equal(Failed, AlwaysFail.Otherwise(AlwaysFail).Execute(new Context()));
        }
        
        [Fact]
        public void SuspendedTask()
        {
            Assert.Throws<TaskSuspendedException>(() => AlwaysSuspended.Otherwise(AlwaysSuspended).Execute(new Context()));
        }
        
        [Fact]
        public void TaskCrashed()
        {
            var c = new Context();
            Assert.Equal(Failed, new CrashingTask().Otherwise(AlwaysSuspended).Execute(c));
            Assert.Single(c.History.Events("TaskException"));
        }
        
        [Fact]
        public void UndoFails()
        {   var command = AlwaysSuccessful.Otherwise(AlwaysFail);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo(new Context()));
        }
        
        [Fact]
        public void UndoCrashes()
        {   var command = AlwaysSuccessful.Otherwise(new CrashingTask());
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Throws<UndoTaskFailureException>(()=>command.Undo(new Context()));
        }

        [Fact]
        public void UndoSuspends()
        {
            var command = AlwaysSuccessful.Otherwise(AlwaysSuspended);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Throws<TaskSuspendedException>(() => command.Undo(new Context()));
        }

        [Fact]
        public void ExecuteTwice()
        {
            var command = new RunOnceTask().Otherwise(AlwaysSuccessful);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Equal(Succeeded, command.Execute(new Context()));
        }

        [Fact]
        public void UndoTwice()
        {
            var command = AlwaysSuccessful.Otherwise(AlwaysSuccessful);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            Assert.Equal(Reverted, command.Undo(new Context()));
            Assert.Throws<InvalidOperationException>(()=> command.Undo(new Context()));
            Assert.Throws<InvalidOperationException>(() => command.Execute(new Context()));
        }

        [Fact]
        public void UndoBeforeExecute()
        {
            var command = AlwaysSuccessful.Otherwise(       AlwaysSuccessful);
            Assert.Throws<InvalidOperationException>(() => command.Undo(new Context()));
        }
    }
}
