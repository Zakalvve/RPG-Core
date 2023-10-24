using Item;
using System;
using UnityEngine;

namespace InventorySystem
{
    public class UI_ActionBarController : UI_InventoryController<IActionBar,IActionItem>, IUI_InventoryController<IActionBar,int>
    {
        public UI_ActionBarController() : base() { }
        public UI_ActionBarController(ActionBar inventory) : base(inventory) { }

        public override void OnClick(int index)
        {
            try
            {
                Inventory.Execute(index);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                _messenger.ShowMessage(e.Message, Color.red);
                base.OnClick(index);
            }
        }
    }
}
