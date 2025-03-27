using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandEventType;

namespace CommandEngine.Commands
{
    internal class StaticAnalyzer : CommandVisitor
    {
        private readonly HashSet<Enum> _outSideLabels = [];
        private readonly HashSet<Enum> _writtenLabels = [];
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
            if (!executeTask.NeededLabels.Any(label => _writtenLabels.Contains(label)))
            {
                _outSideLabels.UnionWith(executeTask.NeededLabels);
            }
            _writtenLabels.UnionWith(executeTask.ChangedLabels);
        }

        public void Visit(CommandHistory history, List<string> events)
        {
            history.Event(SortedList(_outSideLabels), OutSideLabels);
            history.Event(SortedList(_writtenLabels), WrittenLabels);
        }

        private List<string> SortedList(HashSet<Enum> labels) => 
            labels.Select(x => x.ToString()).OrderByDescending(x => x).ToList();
    }
}
