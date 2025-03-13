using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandEngine.Tasks
{
    public class MissingContextInformationException(object missingLabel) : Exception("Missing context information for label: " + missingLabel)
    {

    }
}
