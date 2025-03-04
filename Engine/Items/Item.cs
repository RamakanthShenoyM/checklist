using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Items
{
    public interface Item
    {
        public ItemStatus Status();
        public void Be(object value);
        public void Reset();

    }
}
