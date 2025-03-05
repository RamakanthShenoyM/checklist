using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Engine.Items.ItemStatus;

namespace Engine.Items
{
    public class NotItem : Item
    {
        private readonly Item _item;
        internal NotItem(Item item)
        {
            _item = item;
        }

        public void Be(object value) => _item.Be(value);

        public void Reset() => _item.Reset();

        public ItemStatus Status()
        {
            if (_item.Status() == Succeeded) return Failed;
            if (_item.Status() == Failed) return Succeeded;
            return Unknown;
        }
    }
}
