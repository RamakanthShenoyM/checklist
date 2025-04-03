namespace CommandEngine.Commands
{
    public enum CommandEventType
    {
        StartSubTaskHistory,
        EndSubTaskHistory,
        CommandStateChange,
        TaskExecuted,
        ValueChanged,
        GroupSerialStart,
        GroupSerialComplete,
        TaskException,
        TaskStatus,
        ConclusionReached,
        InvalidAccessAttempt,
        UpdateNotCaptured,
        OutSideLabels,
        WrittenLabels,
        SetAndUsedLabels,
        NeededLabelBeforeSet
    }
}
