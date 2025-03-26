using System.Reflection;
using CommandEngine.Commands;

namespace CommandEngine.Tasks {
    // Defines contract for being able to save and restore a Task
    public interface MementoTask {
        string ToMemento();
        object Clone();
    }
}