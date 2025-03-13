
using CommandEngine.Tasks;

namespace CommandEngine.Commands
{
    public class TaskSuspendedException(CommandTask suspendedTask, SimpleCommand command) 
        : CommandException($"Task {suspendedTask} suspended in command {command}")
    {
        public Command Command => command;
    }
}
