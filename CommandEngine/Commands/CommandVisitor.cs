namespace CommandEngine.Commands
{
    public interface CommandVisitor
    {
        public void PreVisit(SerialCommand command); 
        public void PostVisit(SerialCommand command); 
        public void Visit(SimpleCommand command, CommandState state); 
    }
}
