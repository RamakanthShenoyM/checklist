using static CommandEngine.Commands.EnvironmentSerializer;

namespace CommandEngine.Commands
{
    public static class CommandExtensions
    {
        public static SerialCommand Sequence(this string groupName, Command firstCommand, params Command[] commands) =>
                        new SerialCommand(groupName, firstCommand, commands);

        public static string ToJson(this CommandEnvironment command) => new EnvironmentSerializer(command).Result;

        public static CommandEnvironment ToCommandEnvironment(this string json) => new EnvironmentDeserializer(json).Result;
    }
}
