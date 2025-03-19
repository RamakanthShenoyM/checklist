using CommandEngine.Commands;
using CommandEngine.Tasks;

namespace CommandEngine.Tests.Util {
    internal class StateVisitor : CommandVisitor
    {
        private readonly List<KeyValuePair<SimpleCommand, CommandState>> _states = new();
        
        public StateVisitor(Command command)
        {
            command.Accept(this);
        }

        internal CommandState this[SimpleCommand command] => _states.Find( keyValuePair => keyValuePair.Key == command).Value;

        internal List<CommandState> States => _states.Select( keyValuePair => keyValuePair.Value).ToList();

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask) => 
            _states.Add(new KeyValuePair<SimpleCommand, CommandState>(command, state));
    }
}