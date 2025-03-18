using CommandEngine.Tasks;

namespace CommandEngine.Commands {
    // Understands a textual rendering of a Command hierarchy with Tasks
    public class PrettyPrint : CommandVisitor {
        private int _indentionLevel;
        private string _result = "";

        internal string Result => _result;

        public PrettyPrint(Command command) {
            command.Accept(this);
        }

        public void PreVisit(SerialCommand command, string name, List<Command> subCommands) {
            _result += $"{Indention}{name} Group Command with {subCommands.Count} sub commands\n";
            _indentionLevel++;
        }

        public void PostVisit(SerialCommand command, string name, List<Command> subCommands) {
            _indentionLevel--;
        }

        public void Visit(
            SimpleCommand command,
            CommandState state,
            CommandTask executeTask,
            CommandTask revertTask) {
            _result += $"{Indention}Command with tasks:\n";
            _indentionLevel++;
            _result += $"{Indention}Execution task: {executeTask}\n";
            _result += $"{Indention}Reverting task: {revertTask}\n";
            _indentionLevel--;
        }

        private string Indention => new(' ', _indentionLevel * 2);
    }
}