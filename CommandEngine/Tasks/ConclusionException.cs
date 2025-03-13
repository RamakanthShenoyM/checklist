using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public class ConclusionException(object conclusion) : CommandException("Process Completed with " + conclusion)
    {
        public object Conclusion => conclusion;
    }
}
