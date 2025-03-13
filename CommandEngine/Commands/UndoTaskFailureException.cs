using CommandEngine.Tasks;

namespace CommandEngine.Commands
{
    public class UndoTaskFailureException(CommandTask undoTask, SimpleCommand command) 
        : CommandException($"Undo Failure of task {undoTask} in command {command}") { }
}
