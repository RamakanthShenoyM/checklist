using CommandEngine.Commands;
using CommandEngine.Tasks;

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

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask) => _states[command] = state;
    }
}