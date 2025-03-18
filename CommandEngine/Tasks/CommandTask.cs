using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public interface CommandTask
    {
        CommandStatus Execute(Context c);
        List<object> NeededLabels { get; }
        List<object> ChangedLabels { get; }
        public static CommandTask Ignore => new IgnoreTask();  

        public static CommandTask Mandatory(CommandTask subTask) =>
            new MandatoryTask(subTask);
    }
}