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

    public static class ItemExtensions
    {
        public static NotItem Not(this Item item) => new NotItem(item);
        public static OrItem Or(this Item item1, Item item2) => new OrItem(item1, item2);

    }
}
