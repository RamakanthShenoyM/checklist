namespace CommandEngine.Commands
{
#pragma warning disable CS9113 // Parameter is unread.
    public class UndoTaskFailureException(CommandTask undoTask, SimpleCommand command) : Exception("Undo Failure")
#pragma warning restore CS9113 // Parameter is unread.
    {
    }
}
