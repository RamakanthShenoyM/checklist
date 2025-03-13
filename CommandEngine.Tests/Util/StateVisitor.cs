using CommandEngine.Commands;

namespace CommandEngine.Tests.Util {
    internal class StateVisitor : CommandVisitor
    {
        private readonly OrderedDictionary<SimpleCommand, CommandState> _states = new();
        public StateVisitor(Command command)
        {
            command.Accept(this);
        }

        internal CommandState this[SimpleCommand command] => _states[command];

        internal List<CommandState> States => _states.Values.ToList();

        public void PostVisit(SerialCommand command)
        {
        }

        public void PreVisit(SerialCommand command)
        {
        }

        public void Visit(SimpleCommand command, CommandState state) => _states[command] = state;
    }
}