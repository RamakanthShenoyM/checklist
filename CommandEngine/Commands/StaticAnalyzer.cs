using CommandEngine.Tasks;

namespace CommandEngine.Commands
{
    internal class StaticAnalyzer : CommandVisitor
    {
        private readonly SortedSet<Enum> _outSideLabels = [];
        private readonly SortedSet<Enum> _writtenLabels = [];
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
            history.Event(_outSideLabels.ToList(), CommandEventType.OutSideLabels);
            history.Event(_writtenLabels.ToList(), CommandEventType.WrittenLabels);
        }
    }
}
