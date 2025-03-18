using CommandEngine.Tasks;

namespace CommandEngine.Commands {
    public interface CommandVisitor {
        public void PreVisit(SerialCommand command, string name, List<Command> subCommands) { }
        
        public void PostVisit(SerialCommand command, string name, List<Command> subCommands) { }

        public void Visit(
            SimpleCommand command,
            CommandState state,
            CommandTask executeTask,
            CommandTask revertTask) { }
    }
}