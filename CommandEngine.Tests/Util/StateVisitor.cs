using CommandEngine.Commands;

namespace CommandEngine.Tests.Util {
    internal class StateVisitor : CommandVisitor
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