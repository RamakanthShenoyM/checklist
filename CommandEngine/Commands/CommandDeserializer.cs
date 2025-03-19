using static CommandEngine.Commands.EnvironmentSerializer;

namespace CommandEngine.Commands
{
    internal class CommandDeserializer
    {
        private string _json;

        public CommandDeserializer(string json)
        {
            _json = json;
        }
    }
}