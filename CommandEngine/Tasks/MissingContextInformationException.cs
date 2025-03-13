namespace CommandEngine.Tasks {
    public class MissingContextInformationException(object missingLabel)
        : Exception("Missing context information for label: " + missingLabel) { }
}