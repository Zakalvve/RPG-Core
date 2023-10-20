using Item;
using System.Collections.Generic;

namespace InventorySystem
{
    public class ActionBar : ActionSlotContainer<IActionItem>
    {
        public ActionBar(List<E_InventorySlot> dbSlots,IActionContext context) : base(dbSlots,context) { }
    }
}
