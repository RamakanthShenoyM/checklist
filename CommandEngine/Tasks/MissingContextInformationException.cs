using CommandEngine.Commands;

namespace CommandEngine.Tasks {
	public class MissingContextInformationException(object missingLabel)
		: Exception("Missing context information for label: " + missingLabel)
	{
		public object MissingLabel => missingLabel;
	}
}