namespace CommandEngine.Commands
{
    public interface Command
    {
        public CommandStatus Execute();
        public CommandStatus Undo();
        public void Accept(CommandVisitor visitor);
    }
}
