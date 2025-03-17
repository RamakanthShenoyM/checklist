using CommandEngine.Commands;
using CommandEngine.Tests.Util;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandState;
using static CommandEngine.Commands.SerialCommand;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandEventType;

namespace CommandEngine.Tests.Unit
{
    public class SerialCommandTest
    {
        [Fact]
        public void HappyPath()
        {
            var c = new Context();
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Succeeded, command.Execute(c));
            command.AssertStates(Executed, Executed, Executed);
            Assert.Equal(3, c.History.Events(CommandStateChange).Count);
            Assert.Equal(3,c.History.Events(TaskExecuted).Count);
        }

        

        [Fact]
        public void Suspend()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new SuspendFirstOnly().Otherwise(AlwaysSuccessful)
            );
            var e = Assert.Throws<TaskSuspendedException>(() => command.Execute(new Context()));
            Assert.Equal(command[2], e.Command);
            command.AssertStates(Executed, Executed, Initial);
            Assert.Equal(Succeeded, command.Execute(new Context()));
            command.AssertStates(Executed, Executed, Executed);
        }

        [Fact]
        public void FirstFailure()
        {
            var command = Sequence(
                AlwaysFail.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Failed, command.Execute(new Context()));
            command.AssertStates(FailedToExecute, Initial, Initial);
        }

        [Fact]
        public void SecondFailure()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysFail.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(new Context()));
            command.AssertStates(Reversed, FailedToExecute, Initial);
        }

        [Fact]
        public void ThirdFailure()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(new Context()));
            command.AssertStates(Reversed, Reversed, FailedToExecute);
        }

        [Fact]
        public void SerialWithSerialSuccessful()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful)),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Succeeded, command.Execute(new Context()));
            command.AssertStates(Executed, Executed, Executed, Executed, Executed, Executed);
        }

        [Fact]
        public void SerialWithSerialFailure()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysFail.Otherwise(AlwaysSuccessful)),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(new Context()));
            command.AssertStates(Reversed, Reversed, Reversed, Reversed, FailedToExecute, Initial);
        }

        [Fact]
        public void SerialWithSerialSuspend()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    new SuspendFirstOnly().Otherwise(AlwaysSuccessful)),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<TaskSuspendedException>(() => command.Execute(new Context()));
            command.AssertStates(Executed, Executed, Executed, Executed, Initial, Initial);
            Assert.Equal(Reverted, command.Execute(new Context()));
            command.AssertStates(Reversed, Reversed, Reversed, Reversed, Reversed, FailedToExecute);
        }

        [Fact]
        public void SerialWithSerialCrashed()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(new CrashingTask())),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<UndoTaskFailureException>(() => command.Execute(new Context()));
            command.AssertStates(Executed, Executed, Executed, Executed, FailedToUndo, FailedToExecute);
        }

        [Fact]
        public void SerialWithNoContent()
        {
            Assert.Throws<ArgumentOutOfRangeException>(()=>new SerialCommand(new List<Command>()));
        }
    }
}
