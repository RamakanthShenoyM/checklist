using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public class ConclusionTask(object conclusion) : CommandTask
    {
        public List<object> NeededLabels => new();

        public List<object> ChangedLabels => new();

        public CommandStatus Execute(Context c)
        {
            throw new ConclusionException(conclusion);
        }
    }
}
