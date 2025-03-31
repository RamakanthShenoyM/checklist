using CommandEngine.Commands;
using CommandEngine.Tasks;
using CommandEngine.Tests.Util;
using Xunit.Abstractions;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandState;
using static CommandEngine.Commands.CommandEventType;

namespace CommandEngine.Tests.Unit
{
    public class SerialCommandTest(ITestOutputHelper testOutput)
    {
        [Fact]
        public void Equality()
        {
            var c = new Context();
            var command1 = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );

            var command2 = command1.Clone();
            Assert.Equal(command1, command2);
            Assert.NotEqual(command1, new object());
                #pragma warning disable xUnit2000
            Assert.NotEqual(command1, null);
                #pragma warning restore xUnit2000
            Assert.Equal(command1.GetHashCode(), command2.GetHashCode());
        }

        [Fact]
        public void HappyPath()
        {
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Succeeded, command.Execute(c));
            command.AssertStates(Executed, Executed, Executed);
            Assert.Equal(3, c.History.Events(CommandStateChange).Count);
            Assert.Equal(3, c.History.Events(TaskExecuted).Count);
        }



        [Fact]
        public void Suspend()
        {
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new SuspendFirstOnly().Otherwise(AlwaysSuccessful)
            );
            var e = Assert.Throws<TaskSuspendedException>(() => command.Execute(c));
            Assert.Equal(command[2], e.Command);
            command.AssertStates(Executed, Executed, Initial);
            Assert.Equal(Succeeded, command.Execute(c));
            command.AssertStates(Executed, Executed, Executed);
        }

        [Fact]
        public void FirstFailure()
        {
            var command = "Master Sequence".Sequence(
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
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysFail.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(c));
            command.AssertStates(Reversed, FailedToExecute, Initial);
        }

        [Fact]
        public void ThirdFailure()
        {
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(new Context()));
            command.AssertStates(Reversed, Reversed, FailedToExecute);
        }

        [Fact]
        public void WithoutReverting()
        {
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.NoReverting(),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(c));
            command.AssertStates(Reversed, Reversed, FailedToExecute);
            testOutput.WriteLine(c.History.ToString());
        }

        [Fact]
        public void SerialWithSerialSuccessful()
        {
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Internal Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful)),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Succeeded, command.Execute(new Context()));
            command.AssertStates(Executed, Executed, Executed, Executed, Executed, Executed);
            // testOutput.WriteLine($"{command}");
        }

        [Fact]
        public void SerialWithFailedTask()
        {
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Internal Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysFail.Otherwise(AlwaysSuccessful)),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(c));
            command.AssertStates(Reversed, Reversed, Reversed, Reversed, FailedToExecute, Initial);
            Assert.Equal(9,c.History.Events(CommandEventType.TaskStatus).Count);
            Assert.Single(c.History.Events(CommandEventType.TaskStatus).FindAll(e =>e.Contains("Failed")));
            testOutput.WriteLine(c.History.ToString());
        }

        [Fact]
        public void SerialWithSerialFailure()
        {
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Internal Sequence".Sequence(
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
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Internal Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    new SuspendFirstOnly().Otherwise(AlwaysSuccessful)),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
           
            Assert.Throws<TaskSuspendedException>(() => command.Execute(c));
            command.AssertStates(Executed, Executed, Executed, Executed, Initial, Initial);
            testOutput.WriteLine(c.History.ToString());
            Assert.Equal(Reverted, command.Execute(c));
            command.AssertStates(Reversed, Reversed, Reversed, Reversed, Reversed, FailedToExecute);
           
        }


        [Fact]
        public void SerialWithSerialCrashedUndo()
        {
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Internal Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(new CrashingTask())),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<UndoTaskFailureException>(() => command.Execute(c));
            command.AssertStates(Executed, Executed, Executed, Executed, FailedToUndo, FailedToExecute);
            Assert.Single(c.History.Events(TaskException));
        }

        [Fact]
        public void SerialWithSerialFailedUndo()
        {
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Internal Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysFail)),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<UndoTaskFailureException>(() => command.Execute(c));
            command.AssertStates(Executed, Executed, Executed, Executed, FailedToUndo, FailedToExecute);
            testOutput.WriteLine(c.History.ToString());
            Assert.Equal(7, c.History.Events(CommandEventType.TaskStatus).Count);
        }

        [Fact]
        public void SerialWithSerialCrashedExecute()
        {
            var c = new Context();
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Internal Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    new CrashingTask().Otherwise(AlwaysSuccessful)),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted,command.Execute(c));
            command.AssertStates(Reversed, Reversed, Reversed, Reversed, FailedToExecute, Initial);
            Assert.Single(c.History.Events(TaskException));
        }

        [Fact]
        public void SerialWithNoContent()
        {
            Assert.Throws<ArgumentOutOfRangeException>(()=>new SerialCommand(new List<Command>()));
        }
    }
}
