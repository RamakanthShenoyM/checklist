using static CommandEngine.Commands.CommandSerializer;

namespace CommandEngine.Commands
{
    public static class CommandExtensions
    {
        public static SerialCommand Sequence(this string groupName, Command firstCommand, params Command[] commands) =>
                        new SerialCommand(groupName, firstCommand, commands);

        public static string ToJson(this SerialCommand command) => new CommandSerializer(command).Result;

        public static SerialCommand FromJson(this string json) => new CommandDeserializer(json).Result;
    }
}
