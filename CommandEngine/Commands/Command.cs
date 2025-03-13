using CommandEngine.Tasks;

namespace CommandEngine.Commands
{
    public interface Command
    {
        public CommandStatus Execute(Context c);
        public CommandStatus Undo(Context c);
        public void Accept(CommandVisitor visitor);
        public Command this[int index]=>throw new InvalidOperationException();
    }
}
