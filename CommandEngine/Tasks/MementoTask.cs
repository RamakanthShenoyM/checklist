using System.Reflection;
using CommandEngine.Commands;

namespace CommandEngine.Tasks {
    // Defines contract for being able to save and restore a Task
    public interface MementoTask<T> {
        string ToMemento();
        T Clone();

        public static void ValidateMementoStatus(Type type) { 
            if (!type.NeedsMemento()) return; // No need for Mememento
            if (type.InstanceMethod("Clone") == null) throw new InvalidOperationException(
                    $"Class <{type.Name}> is missing required Clone() method");
            if (type.InstanceMethod("ToMemento") == null) throw new InvalidOperationException(
                    $"Class <{type.Name}> is missing required ToMemento() method");
            if(type.StaticFromMemento() == null) throw new InvalidOperationException(
                $"Class <{type.Name}> is missing required static FromMemento() method");
        }
    }
}