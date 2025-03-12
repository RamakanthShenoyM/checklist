using CommandEngine.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Unit.PermanentStatus;
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

        private class SuspendFirstOnly : CommandTask
        {
            private bool _hasSuspended;
            public CommandStatus Execute()
            {
                if (_hasSuspended)
                    return Succeeded;
                _hasSuspended = true;
                return Suspended;
            }
        }

        private class StateVisitor : CommandVisitor
        {
            private readonly Dictionary<SimpleCommand, CommandState> _states = new();
            public StateVisitor(SerialCommand command)
            {
                command.Accept(this);
            }

            public CommandState this[SimpleCommand command] => _states[command];

            public void PostVisit(SerialCommand command)
            {
            }

            public void PreVisit(SerialCommand command)
            {
            }

            public void Visit(SimpleCommand command, CommandState state) => _states[command] = state;
        }
    }
}
