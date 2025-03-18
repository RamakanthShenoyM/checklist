using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public static class TaskExtensions
    {
        public static SimpleCommand Otherwise(this CommandTask executeTask, CommandTask revertTask) =>
           new SimpleCommand(executeTask, revertTask);   
        public static SimpleCommand NoReverting(this CommandTask executeTask) =>
           new SimpleCommand(executeTask, CommandTask.Ignore);
        public static MandatoryTask Mandatory(this CommandTask task) => new MandatoryTask(task);
    }
}
