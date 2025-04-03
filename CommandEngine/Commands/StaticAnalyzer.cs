using System.Collections;
using CommandEngine.Tasks;
using System.Reflection.Emit;
using CommonUtilities.Util;
using static CommandEngine.Commands.CommandEventType;
using static System.String;

namespace CommandEngine.Commands
{
    internal class StaticAnalyzer : CommandVisitor
    {
        private readonly HashSet<Enum> _outSideLabels = [];
        private readonly HashSet<Enum> _writtenLabels = [];
        private readonly HashSet<Enum> _setAndUsedLabels = [];
        private readonly HashSet<Enum> _beforeNeededlabel = [];
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
            var errorLabels = executeTask.ChangedLabels.FindAll(label => _outSideLabels.Contains(label));
                _beforeNeededlabel.UnionWith(errorLabels);
            var newWrittenLabels = executeTask.ChangedLabels.FindAll(label => !_setAndUsedLabels.Contains(label));
            _writtenLabels.UnionWith(newWrittenLabels);

        }
        public void Visit(
            Context c,
            Dictionary<Enum, object> entries,
            History history)
        { 
            history.Add(OutSideLabels, $"<{Join(", ", SortedList(_outSideLabels))}> are needed from the outside");
            history.Add(WrittenLabels, $"<{Join(", ", SortedList(_writtenLabels.Except(_setAndUsedLabels).ToHashSet()))}> are set for the outside");
            history.Add(SetAndUsedLabels, $"<{Join(", ", SortedList(_setAndUsedLabels))}> are being set and used in the same command environment");
            history.Add(NeededLabelBeforeSet, $"<{Join(", ", SortedList(_beforeNeededlabel))}> set before need");
        }

        private List<string> SortedList(HashSet<Enum> labels) => 
            labels.Select(x => x.ToString()).OrderBy(x => x).ToList();
    }
}
