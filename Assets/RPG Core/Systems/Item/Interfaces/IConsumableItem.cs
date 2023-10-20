using System;

namespace Item
{
    public interface IConsumableItem : IActionItem
    {
        void Consume(Action<int> consume);
    }
}