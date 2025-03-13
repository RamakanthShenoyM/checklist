
namespace CommandEngine.Commands
{
    public class TaskSuspendedException(CommandTask suspendedTask, SimpleCommand command) 
        : Exception($"Task {suspendedTask} suspended in command {command}")
    {
        public Command Command => command;
    }
}
