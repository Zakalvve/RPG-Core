using Item;
using System;
using UnityEngine;

namespace InventorySystem
{
    public class AbilityBookController : UI_BaseInventoryController<ActionSlotContainer<IActionItem>,int>
    {
        public override ISlotData OnDrag(int index)
        {
            try
            {
                if (!IsLinked) throw new NullReferenceException("Controller is not linked with a valid Inventory. Inventory null");
                return Inventory[index];
            }
            catch (Exception e)
            {
                _messenger.ShowMessage(e.Message,Color.red);
                Debug.LogWarning(e.Message);
                return base.OnDrag(index);
            }
        }
    }
}
