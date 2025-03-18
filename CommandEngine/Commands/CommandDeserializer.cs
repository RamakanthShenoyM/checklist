using static CommandEngine.Commands.CommandSerializer;

namespace CommandEngine.Commands
{
    internal class CommandDeserializer
    {
        private string _json;

        public CommandDeserializer(string json)
        {
            _json = json;
        }

        public SerialCommand Result => System.Text.Json.JsonSerializer.Deserialize<SerialCommandDto>(_json).ToCommand();
    }
}