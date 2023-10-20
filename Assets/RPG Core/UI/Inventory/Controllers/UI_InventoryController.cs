using Item;
using System;
using UnityEngine;

namespace InventorySystem
{
    public class UI_InventoryController<TInv, TItem> : UI_BaseInventoryController<TInv,int> where TInv : class, ISlotContainerList<TItem> where TItem : class, IItem
    {
        public UI_InventoryController() : base() { }
        public UI_InventoryController(TInv inventory) : base(inventory) { }
        public override void OnClick(int index)
        {
            if (!IsLinked) return;
            if (Inventory is IActiveInventory<int>)
            {
                try
                {
                    var activeInventory = (IActiveInventory<int>)Inventory;
                    activeInventory.TryExecute(index);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                    _messenger.ShowMessage(e.Message,Color.red);
                }
            }
            else
            {
                base.OnClick(index);
            }
        }
        public override ISlotData OnDrag(int index)
        {
            try
            {
                if (!IsLinked) throw new NullReferenceException("Controller is not linked with a valid Inventory. Inventory null");
                return Inventory.Eject(index);
            }
            catch (Exception e)
            {
                _messenger.ShowMessage(e.Message,Color.red);
                Debug.LogWarning(e.Message);
                return base.OnDrag(index);
            }
        }
        public override ISlotData OnDrop(int index,ISlotData input,out bool wasError)
        {
            try
            {
                if (!IsLinked) throw new NullReferenceException("Controller is not linked with a valid Inventory. Inventory null");
                wasError = false;
                return Inventory.Insert(input,index);
            }
            catch (Exception e)
            {
                wasError = true;
                _messenger.ShowMessage(e.Message,Color.red);
                Debug.LogWarning(e.Message);
                if (!input.IsEmpty) return input;                
                return base.OnDrop(index,input,out wasError);
            }
        }
    }
}
