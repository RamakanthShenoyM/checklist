using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Util
{
    public interface HistoryVisitor
    {
        public void Visit(History history, List<string> events){}
    }
}
