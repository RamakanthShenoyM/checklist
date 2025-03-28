﻿using CommandEngine.Tasks;
using static System.Text.Json.JsonSerializer;
using static CommandEngine.Commands.CommandEnvironment;
using static CommandEngine.Commands.EnvironmentSerializer;
using static CommandEngine.Commands.CommandReflection;

namespace CommandEngine.Commands {
    internal class EnvironmentDeserializer : CommandVisitor {
        private readonly CommandEnvironment _environment;
        private readonly List<SimpleCommandDto> _simpleCommandDtos;

        public EnvironmentDeserializer(string json) {
            try {
                var dto = Deserialize<ExtensionDto>(json) ?? throw new InvalidOperationException("Invalid Json");
                _simpleCommandDtos = dto.SimpleCommandDtos;
                _environment = RestoredEnvironment(
                    Environment(new Guid(dto.EnvironmentId)),
                    new Guid(dto.ClientId),
                    Context(dto)
                );
                _environment.Accept(this);
            }
            catch (InvalidOperationException) {
                throw;
            }
            catch (Exception e) {
                throw new InvalidOperationException($"Error deserializing environment: {e.Message}", e);
            }
        }

        private static Context Context(ExtensionDto dto) {
            var c = new Context(dto.Events);
            foreach (var entry in dto.Entries)
                c[Label(entry.EnumType, entry.EnumValue)] = Value(entry.ValueType, entry.ValueValue);
            return c;
        }

        private static object Value(string entryValueType, string entryValueValue) {
			Type valueType = FoundType(entryValueType);
			if (valueType.IsEnum)
				return Enum.Parse(valueType, entryValueValue);

			return Convert.ChangeType(entryValueValue, valueType);
        }

        private static Enum Label(string enumTypeName, string enumValue) {
            Type enumType = FoundType(enumTypeName);
            if (enumType == null || !enumType.IsEnum) throw new InvalidOperationException(
                    $"Type '{enumTypeName}' is not an enum.");
            return (Enum)Enum.Parse(enumType, enumValue);
        }

       

        public CommandEnvironment Result => _environment;

        public void Visit(SimpleCommand command, CommandState state, CommandTask executeTask, CommandTask revertTask) {
            command.State(_simpleCommandDtos[0].State);
            if (_simpleCommandDtos[0].ExecuteTask.Memento != null)
                command.ExecuteTask(Task(_simpleCommandDtos[0].ExecuteTask));
            _simpleCommandDtos.RemoveAt(0);
        }

        private CommandTask Task(TaskDto dto)
        {
            var type = FoundType(dto.TaskType);
            return (CommandTask)type.StaticFromMemento().Invoke(type, [dto.Memento]);
        }
    }
}