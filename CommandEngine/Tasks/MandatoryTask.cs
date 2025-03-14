using CommandEngine.Commands;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Tasks
{
    public class MandatoryTask(CommandTask subTask) : CommandTask
    {
        public List<object> NeededLabels => subTask.NeededLabels;

        public List<object> ChangedLabels => subTask.ChangedLabels;

        public CommandStatus Execute(Context c)
        {
            if(NeededLabels.All(label=> c.Has(label))) return subTask.Execute(c);
            return Suspended;
        }
    }
}