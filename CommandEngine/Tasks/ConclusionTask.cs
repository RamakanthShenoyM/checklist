using CommandEngine.Commands;
using static CommandEngine.Tasks.CommandTask;

namespace CommandEngine.Tasks
{
    public class ConclusionTask(object conclusion) : CommandTask
    {
        public CommandStatus Execute(Context c)
        {
            throw new ConclusionException(conclusion);
        }
    }
}
