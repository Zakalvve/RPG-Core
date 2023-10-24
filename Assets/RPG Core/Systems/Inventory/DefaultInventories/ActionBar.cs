using Item;
using System.Collections.Generic;

namespace InventorySystem
{
    public class ActionBar : ActionSlotContainer<IActionItem>, IActionBar
    {
        public ActionBar(List<E_InventorySlot> dbSlots,IActionContext context) : base(dbSlots,context) { }
    }

    public interface IActionBar : ISlotContainerList<IActionItem>
    {
        void Execute(int index);
    }
}
