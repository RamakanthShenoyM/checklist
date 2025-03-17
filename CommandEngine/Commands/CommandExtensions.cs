using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandEngine.Commands
{
    public static class CommandExtensions
    {
        public static SerialCommand Sequence(this string groupName, Command firstCommand, params Command[] commands) =>
                        new SerialCommand(groupName, firstCommand, commands);
    }
}
