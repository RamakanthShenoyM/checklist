using CommandEngine.Commands;
using static CommandEngine.Commands.CommandEnvironment;
using static System.Text.Json.JsonSerializer;
using static CommandEngine.Commands.CommandReflection;

namespace CommandEngine.Tasks {
    public class CommandTaskWrapper(CommandEnvironment environment, List<Enum> neededLabels, List<Enum> changedLabels)
        : CommandTask, MementoTask {
        private readonly CommandEnvironment _environment = environment;

        public List<Enum> NeededLabels => neededLabels;

        public List<Enum> ChangedLabels => changedLabels;

        public CommandTask Clone() =>
            new CommandTaskWrapper(FreshEnvironment(_environment), NeededLabels, ChangedLabels);

        public CommandStatus Execute(Context c) => _environment.Execute(c);

        public string? ToMemento() =>
            Serialize(new CommandWrapperDto(
                Labels(NeededLabels),
                Labels(ChangedLabels),
                _environment.ToMemento()
            ));

        private List<LabelDto> Labels(List<Enum> labels) =>
            labels.Select(label => new LabelDto(
                label.GetType().FullName ?? throw new InvalidOperationException(
                    "Missing required value for Context Label type in DTO"),
                label.ToString()
            )).ToList();

        private static List<Enum> Labels(List<LabelDto> labels) =>
            labels.Select(label => Label(label.EnumType, label.EnumValue)).ToList();

        private static Enum Label(string enumTypeName, string enumValue) {
            Type enumType = FoundType(enumTypeName);
            if (enumType is not { IsEnum: true })
                throw new InvalidOperationException( $"Type '{enumTypeName}' is not an enum.");
            return (Enum)Enum.Parse(enumType, enumValue);
        }

        public static CommandTaskWrapper FromMemento(string memento) {
            var dto = Deserialize<CommandWrapperDto>(memento) 
                      ?? throw new InvalidOperationException("Invalid Json");
            var environment = CommandEnvironment.FromMemento(dto.EnvironmentMemento);
            return new CommandTaskWrapper(environment, Labels(dto.NeededLabels), Labels(dto.ChangedLabels));
        }

        public override string ToString() => this.GetType().Name;

        public override bool Equals(object? obj) =>
            this == obj || obj is CommandTaskWrapper other && this.Equals(other);

        private bool Equals(CommandTaskWrapper other) =>
            this.NeededLabels.SequenceEqual(other.NeededLabels) &&
            this.ChangedLabels.SequenceEqual(other.ChangedLabels) &&
            this._environment.Equals(other._environment);

        public override int GetHashCode() => HashCode.Combine(this.NeededLabels, this.ChangedLabels);
    }

    internal record LabelDto(string EnumType, string EnumValue) { }

    internal record CommandWrapperDto(
        List<LabelDto> NeededLabels,
        List<LabelDto> ChangedLabels,
        string EnvironmentMemento) { }
}