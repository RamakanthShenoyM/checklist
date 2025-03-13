using CommandEngine.Commands;
using CommandEngine.Tests.Util;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandState;

namespace CommandEngine.Tests.Unit
{
    public class SerialCommandTest 
    {
        [Fact]
        public void HappyPath()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Equal(Succeeded, command.Execute());
            command.AssertStates(Executed, Executed, Executed);
        }

        [Fact]
        public void Suspend()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new SuspendFirstOnly().Otherwise(AlwaysSuccessful)
            }.ToCommand();
            var e=Assert.Throws<TaskSuspendedException>(()=> command.Execute());
            Assert.Equal(command[2], e.Command);
            command.AssertStates(Executed, Executed, Initial);
            Assert.Equal(Succeeded, command.Execute());
            command.AssertStates(Executed, Executed, Executed);
        }

        [Fact]
        public void FirstFailure()
        {
            var command = new Command[]
            {
                AlwaysFail.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Equal(Failed, command.Execute());
            command.AssertStates(FailedToExecute, Initial, Initial);
        }
        
        [Fact]
        public void SecondFailure()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysFail.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Equal(Reverted, command.Execute());
            command.AssertStates(Reversed, FailedToExecute, Initial);
        }
        
        [Fact]
        public void ThirdFailure()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Equal(Reverted, command.Execute());
            command.AssertStates(Reversed, Reversed, FailedToExecute);
        }

        [Fact]
        public void SerialWithSerialSuccessful()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful), 
                new Command[]
                {
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful)
                }.ToCommand(),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Equal(Succeeded, command.Execute());
            command.AssertStates(Executed,Executed, Executed, Executed, Executed, Executed);
        } 
        
        [Fact]
        public void SerialWithSerialFailure()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful), 
                new Command[]
                {
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysFail.Otherwise(AlwaysSuccessful)
                }.ToCommand(),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Equal(Reverted, command.Execute());
            command.AssertStates(Reversed, Reversed, Reversed, Reversed, FailedToExecute, Initial);
        } 
        
        [Fact]
        public void SerialWithSerialSuspend()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful), 
                new Command[]
                {
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    new SuspendFirstOnly().Otherwise(AlwaysSuccessful)
                }.ToCommand(),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Throws<TaskSuspendedException>(() => command.Execute());
            command.AssertStates(Executed, Executed, Executed, Executed, Initial, Initial);
            Assert.Equal(Reverted, command.Execute());
            command.AssertStates(Reversed, Reversed, Reversed, Reversed, Reversed, FailedToExecute);
        }

        [Fact]
        public void SerialWithSerialCrashed()
        {
            var command = new Command[]
            {
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new Command[]
                {
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(new CrashingTask())
                }.ToCommand(),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            }.ToCommand();
            Assert.Throws<UndoTaskFailureException>(() => command.Execute());
            command.AssertStates(Executed, Executed, Executed, Executed, FailedToUndo, FailedToExecute);
        }
    }
}
