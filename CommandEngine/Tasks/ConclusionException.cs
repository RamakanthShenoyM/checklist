namespace CommandEngine.Tasks
{
    public class ConclusionException(object conclusion) : Exception("Process Completed with " + conclusion)
    {
        public object Conclusion => conclusion;
    }
}
