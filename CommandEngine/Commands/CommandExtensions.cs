namespace CommandEngine.Commands
{
    public static class CommandExtensions
    {
        public static SerialCommand Sequence(this string groupName, Command firstCommand, params Command[] commands) =>
                        new SerialCommand(groupName, firstCommand, commands);
    }
}
