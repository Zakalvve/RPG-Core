using Item;
using System;
using System.Collections.Generic;

namespace InventorySystem
{
    public class ActionSlotContainer<TItem> : SlotContainer<TItem>, IActionSlotContainer<int> where TItem : class, IActionItem
    {
        private IActionContext _context;
        public ActionSlotContainer(List<E_InventorySlot> dbSlots,IActionContext context) : base(dbSlots) => _context = context;
        public override ISlotData Insert(ISlotData slot)
        {
            if (!IsTItem(slot.Item)) throw new ArgumentException("You can't put that there.");
            return base.Insert(slot);
        }
        public override ISlotData Insert(ISlotData slot,int at)
        {
            if (!IsTItem(slot.Item)) throw new ArgumentException("You can't put that there.");
            return base.Insert(slot,at);
        }
        public override ISlotData Eject(int from,int stacksToRemove = int.MaxValue)
        {
            return base.Eject(from,stacksToRemove);
        }
#nullable enable
        private bool IsTItem(IItem? item)
        {
            return item is TItem;
        }
#nullable disable
        public virtual void Execute(int index)
        {
            var item = this[index].Item;
            if (item != null && item is TItem)
            {
                if (item is IConsumableItem)
                {
                    var consumable = (IConsumableItem)item;
                    consumable.Use(_context);
                    consumable.Consume((amount) => Eject(index,amount));
                }
                else
                {
                    ((TItem)item).Use(_context);
                }
            }
        }
    }
}