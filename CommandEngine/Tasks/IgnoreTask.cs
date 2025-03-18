using CommandEngine.Commands;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Tasks
{
    public class IgnoreTask: CommandTask
    {
        internal IgnoreTask() { 
        }

        public List<object> NeededLabels => [];

        public List<object> ChangedLabels => [];

        public CommandStatus Execute(Context c) => Succeeded;
    }
}
