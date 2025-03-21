using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public class ConclusionTask(object conclusion) : CommandTask
    {
        public List<Enum> NeededLabels => new();

        public List<Enum> ChangedLabels => new();

        public CommandStatus Execute(Context c)
        {
            throw new ConclusionException(conclusion);
        }
    }
}
