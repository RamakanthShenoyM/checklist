﻿using CommandEngine.Tasks;
using System.Collections.Concurrent;

namespace CommandEngine.Commands
{
    public class CommandEnvironment
    {
        private readonly string _name;
        private readonly Command _command;
        private readonly Guid _environmentId;
        private readonly Guid _clientId;
        private Context _context;
        private static readonly ConcurrentDictionary<Guid, CommandEnvironment> _templates = new();

        public Guid EnvironmentId => _environmentId;

        public Guid ClientId => _clientId;

        private CommandEnvironment(string name, Command serialCommand, Guid? environmentId = null, Guid? clientId = null, Context? c = null)
        {
            _name = name;
            _command = serialCommand;
            _environmentId = environmentId ?? Guid.NewGuid();
            _clientId = clientId ?? Guid.NewGuid();
            _context = c ?? new Context([]);

            if (!_templates.ContainsKey(_environmentId))
                _templates[_environmentId] = this;
        }
        
        internal static CommandEnvironment Template(string name, Command command)
        {
            var result= new CommandEnvironment(name, command);
            new StaticAnalyzer(result);
            return result;

        }

        public static CommandEnvironment FreshEnvironment(CommandEnvironment template, Context? c = null)
        {
            var context = c ?? new Context();
            return new CommandEnvironment(template._name, template._command.Clone(), template.EnvironmentId, c: context.Merge(template._context));
        }

        public static CommandEnvironment RestoredEnvironment(CommandEnvironment template, Guid clientId, Context c) =>
            new(template._name, template._command.Clone(), template.EnvironmentId, clientId, c);
        
        public override bool Equals(object? obj) =>
            this == obj || obj is CommandEnvironment other && this.Equals(other);

        public override int GetHashCode() => EnvironmentId.GetHashCode() * 37 + ClientId.GetHashCode();

        private bool Equals(CommandEnvironment other) =>
            this.EnvironmentId == other.EnvironmentId
            && this.ClientId == other.ClientId
            && this._command.Equals(other._command)
            && this._context.Equals(other._context);

        public CommandStatus Execute() => _command.Execute(_context);
		internal CommandStatus Execute(Context c)
		{
            _context = c;
			return Execute();
		}

		public object this[Enum label] => _context[label];
        
        public void Accept(CommandVisitor visitor)
        {
            visitor.PreVisit(this, EnvironmentId, ClientId, _command, _context);
            _command.Accept(visitor);
            _context.Accept(visitor);
            visitor.PostVisit(this, EnvironmentId, ClientId, _command, _context);
        }

        internal static CommandEnvironment Environment(Guid guid)
        {
            if (_templates.ContainsKey(guid))
                return _templates[guid];
            throw new InvalidOperationException($"Environment with id {guid} not found");
        }

        public static CommandEnvironment FromMemento(string memento) =>
            new EnvironmentDeserializer(memento).Result;

        public bool Reset(Enum label) => _context.Reset(label);
        
        public string ToMemento()=> new EnvironmentSerializer(this).Result;

        public CommandEnvironment Clone() => this;

	}
}
