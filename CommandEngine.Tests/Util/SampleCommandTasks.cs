using CommandEngine.Commands;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;


namespace CommandEngine.Tests.Util
{

    internal class PermanentStatus(CommandStatus status) : CommandTask {
        internal static readonly PermanentStatus AlwaysSuccessful = new(Succeeded);
        internal static readonly PermanentStatus AlwaysFail = new(Failed);
        internal static readonly PermanentStatus AlwaysSuspended = new(Suspended);

        public CommandStatus Execute(Context c) => status;
    }

    internal class CrashingTask : CommandTask {
        public CommandStatus Execute(Context c) => throw new InvalidOperationException("unable to execute this task");
    }

    internal class RunOnceTask : CommandTask {
        private bool _hasRun;

        public CommandStatus Execute(Context c)
        {
            if (_hasRun) throw new InvalidOperationException("unable to execute this task twice");
            _hasRun = true;
            return Succeeded;
        }
    }

    internal class SuspendFirstOnly : CommandTask {
        private bool _hasSuspended;

        public CommandStatus Execute(Context c)
        {
            if (_hasSuspended) return Succeeded;
            _hasSuspended = true;
            return Suspended;
        }
    }
}