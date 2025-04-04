using CommonUtilities.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Items;

namespace Engine.Tests.Util
{
    internal class HistoryDump : ChecklistVisitor
    {
        private History? _history;
        internal History Result => _history ?? throw new InvalidOperationException("Visit Failure");
        internal HistoryDump(Checklist checklist)
        {
            checklist.Accept(this);
        }

        public void Visit(History history, List<string> events)
        {
            _history = history;
        }

    }
}
