using CommandEngine.Tasks;

namespace CommandEngine.Commands
{
    public class CommandEnvironment
    {
        private readonly Command _command;
        private readonly Guid _environmentId;
        private readonly Guid _clientId;
        private readonly Context _context;
        private static readonly Dictionary<Guid, CommandEnvironment> _environments = new();

        public Guid EnvironmentId => _environmentId;

        public Guid ClientId => _clientId;


        private CommandEnvironment(Command serialCommand, Guid? environmentId = null, Guid? clientId = null, Context? c = null)
        {
            _command = serialCommand;
            _environmentId = environmentId ?? Guid.NewGuid();
            _clientId = clientId ?? Guid.NewGuid();
            _context = c ?? new Context([]);

            if (!_environments.ContainsKey(_environmentId))
                _environments.Add(_environmentId, this);
        }
        public static CommandEnvironment Template(Command command) => new CommandEnvironment(command);

        public static CommandEnvironment FreshEnvironment(CommandEnvironment template) =>
            new CommandEnvironment(template._command.Clone(), template.EnvironmentId);
        public static CommandEnvironment RestoredEnvironment(CommandEnvironment template, Guid clientId, Context c) =>
            new CommandEnvironment(template._command.Clone(), template.EnvironmentId, clientId, c);
        public override bool Equals(object? obj) =>
            this == obj || obj is CommandEnvironment other && this.Equals(other);

        public override int GetHashCode() => EnvironmentId.GetHashCode() * 37 + ClientId.GetHashCode();

        private bool Equals(CommandEnvironment other) =>
            this.EnvironmentId == other.EnvironmentId
            && this.ClientId == other.ClientId
            && this._command.Equals(other._command)
            && this._context.Equals(other._context);


        public CommandStatus Execute() => _command.Execute(_context);

        internal void Accept(CommandVisitor visitor)
        {
            visitor.PreVisit(this, EnvironmentId, ClientId, _command, _context);
            _command.Accept(visitor);
            _context.Accept(visitor);
            visitor.PostVisit(this, EnvironmentId, ClientId, _command, _context);
        }

        internal static CommandEnvironment Environment(Guid guid)
        {
            if (_environments.ContainsKey(guid))
                return _environments[guid];
            throw new InvalidOperationException($"Environment with id {guid} not found");
        }
    }
}
