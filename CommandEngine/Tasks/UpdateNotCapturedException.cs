
namespace CommandEngine.Tasks
{
    [Serializable]
    internal class UpdateNotCapturedException(object changedLabel) : 
        Exception($"Attempt to set <{changedLabel}> in the context, but not marked as a change field")
    {
        public object ChangedLabel => changedLabel;
    }
}
