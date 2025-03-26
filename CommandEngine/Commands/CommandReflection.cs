using System.Reflection;
using CommandEngine.Tasks;

namespace CommandEngine.Commands {
    
    public static class CommandReflection {
        internal static Type FoundType(string fullTypeName) {
            // Check all currently loaded assemblies in the AppDomain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                var type = assembly.GetType(fullTypeName);
                if (type != null) return type;
            }

            throw new InvalidOperationException($"Type {fullTypeName} not found");
        }

        public static void ValidateMementoStatus(Type type) { 
            if (!type.NeedsMemento()) return; // No need for Mememento
            if (!typeof(MementoTask).IsAssignableFrom(type)) throw new InvalidOperationException(
                $"Class <{type.Name}> does not implement required MementoTask interface");
            if (type.StaticFromMemento() == null) throw new InvalidOperationException(
                $"Class <{type.Name}> is missing required static FromMemento() method");
            if(!type.HasEquals()) throw new InvalidOperationException(
                $"Class <{type.Name}> is missing required override of Equals() method");
        }
        
        internal static Type ToType(this string fullTypeName) => FoundType(fullTypeName);

        internal static bool HasExplicitEquals(Type type) {
            var equalsMethod = type.GetMethod("Equals", [typeof(object)]);
            return equalsMethod != null && equalsMethod.DeclaringType != typeof(object);
        }
        
        internal static bool HasEquals(this Type type) => HasExplicitEquals(type);

        internal static MethodInfo? Method(Type type, string methodName, BindingFlags flag) => 
            type.GetMethod(methodName, flag | BindingFlags.Public);
        
        internal static MethodInfo? InstanceMethod(this Type type, string methodName) =>  
            Method(type, methodName, BindingFlags.Instance);

        internal static bool HasInstanceMethod(this Type type, string methodName) =>
            type.InstanceMethod(methodName) != null;
        
        internal static MethodInfo? StaticFromMemento(this Type type) =>
            type.GetMethod(
                "FromMemento",
                BindingFlags.Public | BindingFlags.Static,
                null,
                [typeof(string)],
                null
            );

        internal static bool HasFromMemento(this Type type) => StaticFromMemento(type) != null;

        internal static bool NeedsMemento(this Type type) {
            var nonPublicFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var publicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            return nonPublicFields.Length > 0 || publicFields.Length > 0;
        }
    }
}