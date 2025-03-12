using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandEngine.Commands
{
    public class TaskSuspendedException(CommandTask suspendedTask, SimpleCommand command) : Exception("Task suspended")
    {
    }
}
