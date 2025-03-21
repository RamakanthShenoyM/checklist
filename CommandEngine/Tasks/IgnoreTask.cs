using CommandEngine.Commands;
using static CommandEngine.Commands.CommandStatus;

namespace CommandEngine.Tasks
{
    public class IgnoreTask: CommandTask
    {
        internal IgnoreTask() { 
        }

        public List<Enum> NeededLabels => [];

        public List<Enum> ChangedLabels => [];

        public CommandStatus Execute(Context c) => Succeeded;
    }
}
