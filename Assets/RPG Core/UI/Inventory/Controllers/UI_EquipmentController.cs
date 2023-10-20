using Item;
using System;
using UnityEngine;
using RPGCore.Item;

namespace InventorySystem
{
    public class UI_EquipmentController<TInv, TItem> : UI_BaseInventoryController<TInv,string> where TInv : class, IEquipmentSlotContainer<EquipmentTypes> where TItem : class, IItem
    {
        public UI_EquipmentController() : base() { }
        public UI_EquipmentController(TInv inventory) : base(inventory) { }
        public override void OnClick(string index)
        {
            base.OnClick(index);
        }
        public override ISlotData OnDrag(string index)
        {
            try
            {
                if (!IsLinked) throw new NullReferenceException("Controller is not linked with a valid Inventory. Inventory null");
                return Inventory.UnEquip(index);
            }
            catch (Exception e)
            {
                _messenger.ShowMessage(e.Message,Color.red);
                Debug.LogWarning(e.Message);
                return base.OnDrag(index);
            }
        }
        public override ISlotData OnDrop(string index,ISlotData input,out bool wasError)
        {
            try
            {
                if (!IsLinked) throw new NullReferenceException("Controller is not linked with a valid Inventory. Inventory null");
                wasError = false;
                return Inventory.Equip(input,index);
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
