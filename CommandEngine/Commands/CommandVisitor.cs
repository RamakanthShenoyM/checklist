﻿using CommandEngine.Tasks;
using CommonUtilities.Util;


namespace CommandEngine.Commands {
    
    public interface CommandVisitor : HistoryVisitor {
        public void PreVisit(CommandEnvironment environment, Guid environmentId, Guid clientId, Command command, Context c) { }

        public void PostVisit(CommandEnvironment environment, Guid environmentId, Guid clientId, Command command, Context c) { }
        public void PreVisit(SerialCommand command, string name, List<Command> subCommands) { }

        public void PostVisit(SerialCommand command, string name, List<Command> subCommands) { }

        public void Visit(
            SimpleCommand command,
            CommandState state,
            CommandTask executeTask,
            CommandTask revertTask)
        { }
        public void Visit(
			Context c,
			Dictionary<Enum, object> entries, 
            History history)
		{ }

    }
}