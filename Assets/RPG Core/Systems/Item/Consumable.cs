using Item;
using System;
using UnityEngine;

namespace Item
{
    public class Consumable : BaseItem<E_Consumable>, IConsumableItem
    {
        public Consumable(E_Consumable data) : base(data) { }

        public void Consume(Action<int> consume)
        {
            consume(_data.f_ConsumeAmount);
        }

        public void Use(IActionContext context)
        {
            context.Use(this);
        }
    }
}
