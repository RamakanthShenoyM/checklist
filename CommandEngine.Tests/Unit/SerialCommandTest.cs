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
            var first = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var second = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var third = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var command = new SerialCommand(first,second,third);
            Assert.Equal(Succeeded, command.Execute());
            var states = new StateVisitor(command);
            Assert.Equal(Executed, states[first]);
            Assert.Equal(Executed, states[second]);
            Assert.Equal(Executed, states[third]);
        }

        [Fact]
        public void Suspend()
        {
            var first = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var second = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var third = new SimpleCommand(new SuspendFirstOnly(), AlwaysSuccessful);
            var command = new SerialCommand(first, second, third);
            var e = Assert.Throws<TaskSuspendedException>(()=> command.Execute());
            Assert.Equal(third, e.Command);
            var states = new StateVisitor(command);
            Assert.Equal(Executed, states[first]);
            Assert.Equal(Executed, states[second]);
            Assert.Equal(Initial, states[third]);
            
            Assert.Equal(Succeeded, command.Execute());

            states = new StateVisitor(command);
            Assert.Equal(Executed, states[first]);
            Assert.Equal(Executed, states[second]);
            Assert.Equal(Executed, states[third]);
        }

        [Fact]
        public void FirstFailure()
        {
            var first = new SimpleCommand(AlwaysFail, AlwaysSuccessful);
            var second = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var third = new SimpleCommand(AlwaysFail, AlwaysSuccessful);
            var command = new SerialCommand(first, second, third);
            Assert.Equal(Failed, command.Execute());
            var states = new StateVisitor(command);
            Assert.Equal(Executed, states[first]);
            Assert.Equal(Initial, states[second]);
            Assert.Equal(Initial, states[third]);
        }
        
        [Fact]
        public void SecondFailure()
        {
            var first = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var second = new SimpleCommand(AlwaysFail, AlwaysSuccessful);
            var third = new SimpleCommand(AlwaysFail, AlwaysSuccessful);
            var command = new SerialCommand(first, second, third);
            Assert.Equal(Reverted, command.Execute());
            var states = new StateVisitor(command);
            Assert.Equal(Reversed, states[first]);
            Assert.Equal(Executed, states[second]);
            Assert.Equal(Initial, states[third]);
        }
        
        [Fact]
        public void ThirdFailure()
        {
            var first = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var second = new SimpleCommand(AlwaysSuccessful, AlwaysSuccessful);
            var third = new SimpleCommand(AlwaysFail, AlwaysSuccessful);
            var command = new SerialCommand(first, second, third);
            Assert.Equal(Reverted, command.Execute());
            var states = new StateVisitor(command);
            Assert.Equal(Reversed, states[first]);
            Assert.Equal(Reversed, states[second]);
            Assert.Equal(Executed, states[third]);
        }
    }
}
