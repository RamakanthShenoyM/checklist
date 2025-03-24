using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandEngine.Commands
{
    internal class CommandReflection
    {
        internal static Type FoundType(string fullTypeName)
        {
            // Check all currently loaded assemblies in the AppDomain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null) return type;
            }
            throw new InvalidOperationException($"Type {fullTypeName} not found");
        }
    }
}
