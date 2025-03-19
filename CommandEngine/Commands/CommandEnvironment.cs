using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandEngine.Tasks;

namespace CommandEngine.Commands
{
    public class CommandEnvironment
    {
        private readonly Command _command;
        private readonly Guid _environmentId;
        private readonly Guid _clientId;
        private readonly Context _context;

        private CommandEnvironment(Command serialCommand, Guid environmentId = new Guid(), Guid clientId = new Guid(), Context? c = null)
        {
            _command = serialCommand;
            _environmentId = environmentId;
            _clientId = clientId;
            _context = c ?? new Context();
        }
        public static CommandEnvironment Template(Command command) => new CommandEnvironment(command);

        public static CommandEnvironment FreshEnvironment(CommandEnvironment template) =>
            new CommandEnvironment(template._command.Clone(), template._environmentId);
        public static CommandEnvironment RestoredEnvironment(CommandEnvironment template, Guid clientId, Context c) =>
            new CommandEnvironment(template._command.Clone(), template._environmentId, clientId, c);
        public override bool Equals(object? obj) =>
            this == obj || obj is CommandEnvironment other && this.Equals(other);

        public override int GetHashCode() => _environmentId.GetHashCode() * 37 + _clientId.GetHashCode();
        private bool Equals(CommandEnvironment other) =>
                this._environmentId == other._environmentId
                && this._clientId == other._clientId
                && this._command .Equals( other._command);

        public CommandStatus Execute() => _command.Execute(_context);
    }
}
