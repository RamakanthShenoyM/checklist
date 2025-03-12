namespace CommandEngine.Commands
{
#pragma warning disable CS9113 // Parameter is unread.
    public class TaskSuspendedException(CommandTask suspendedTask, SimpleCommand command) : Exception("Task suspended")
#pragma warning restore CS9113 // Parameter is unread.
    {
    }
}
