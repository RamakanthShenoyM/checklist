using static CommandEngine.Commands.EnvironmentSerializer;

namespace CommandEngine.Commands {
    public static class CommandExtensions {
        public static SerialCommand Sequence(this string groupName, Command firstCommand, params Command[] commands) =>
            new(groupName, firstCommand, commands);

        public static CommandEnvironment Template(this string name, SerialCommand command) =>
            CommandEnvironment.Template(name, command);
    }
}