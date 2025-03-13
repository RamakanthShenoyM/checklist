namespace CommandEngine.Commands
{
    public class UndoTaskFailureException(CommandTask undoTask, SimpleCommand command) 
        : Exception($"Undo Failure of task {undoTask} in command {command}") { }
}
