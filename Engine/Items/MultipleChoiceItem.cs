using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Items
{
    public class MultipleChoiceItem: Item
    {
        private readonly List<object> _choices;
        private object? _value;
        public MultipleChoiceItem(params object[] choices)
        {
            _choices = choices.ToList();
        }

        public void Be(object value) => _value = value;

        public void Reset() => _value = null;

        public ItemStatus Status()
        {
            if (_value == null) return ItemStatus.Unknown;
            return _choices.Contains(_value)? ItemStatus.Succeeded: ItemStatus.Failed;
        }
    }
}
