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
            Assert.Equal(Succeeded, command.Execute());
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
