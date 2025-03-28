using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandEventType;

namespace CommandEngine.Commands
{
    internal class StaticAnalyzer : CommandVisitor
    {
        private readonly HashSet<Enum> _outSideLabels = [];
        private readonly HashSet<Enum> _writtenLabels = [];
        private readonly HashSet<Enum> _setAndUsedLabels = [];
        public StaticAnalyzer(CommandEnvironment environment)
        {
            environment.Accept(this);
        }

        public void Visit(
            SimpleCommand command,
            CommandState state,
            CommandTask executeTask,
            CommandTask revertTask)
        {
            var usedLabels = executeTask.NeededLabels
                .FindAll(label => _writtenLabels.Contains(label));
            _setAndUsedLabels.UnionWith(usedLabels);
            var newNeededLabels = new List<Enum>(executeTask.NeededLabels).Except(usedLabels);
            _outSideLabels.UnionWith(newNeededLabels);
            var newWrittenLabels = executeTask.ChangedLabels.FindAll(label => !_setAndUsedLabels.Contains(label));
            _writtenLabels.UnionWith(newWrittenLabels);
        }

        public void Visit(CommandHistory history, List<string> events)
        {
           
            history.Event(SortedList(_outSideLabels), OutSideLabels);
            history.Event(SortedList(_writtenLabels.Except(_setAndUsedLabels).ToHashSet()), WrittenLabels);
            history.Event(SortedList(_setAndUsedLabels), SetAndUsedLabels);
        }

        private List<string> SortedList(HashSet<Enum> labels) => 
            labels.Select(x => x.ToString()).OrderBy(x => x).ToList();
    }
}
