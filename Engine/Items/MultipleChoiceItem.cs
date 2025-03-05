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
        public MultipleChoiceItem(object firstChoice, params object[] choices)
        {
            _choices = choices.ToList();
            _choices.Insert(0, firstChoice);
        }

        public override void Be(object value) => _value = value;

        public override void Reset() => _value = null;

        internal override ItemStatus Status()
        {
            if (_value == null) return ItemStatus.Unknown;
            return _choices.Contains(_value)? ItemStatus.Succeeded: ItemStatus.Failed;
        }
    }
}
