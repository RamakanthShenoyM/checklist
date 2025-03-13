namespace CommandEngine.Commands
{
    public interface Command
    {
        public CommandStatus Execute();
        public CommandStatus Undo();
        public void Accept(CommandVisitor visitor);
        public Command this[int index]=>throw new InvalidOperationException();
    }
}
