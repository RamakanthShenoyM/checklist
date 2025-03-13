using CommandEngine.Commands;

namespace CommandEngine.Tasks
{
    public interface CommandTask
    {
        public CommandStatus Execute(Context c);
        public readonly static object Conclusion = new();
    }
}