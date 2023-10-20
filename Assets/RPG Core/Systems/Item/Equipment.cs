using InventorySystem;
using RPGCore.Item;
using System;
using UnityEngine;

namespace Item
{
    //Equipment always has a max stack size of 1, its unstackable
    [Serializable]
    public class Equipment : BaseItem<E_Equipment>, IEquipableItem<EquipmentTypes>
    {
        public Equipment(E_Equipment data) : base(data) { }

        public EquipmentTypes f_ValidEquipmentSlot => _data.f_ValidEquipmentSlot;
        public void Equip(IEquipContext<EquipmentTypes> context)
        {
            context.Equip(this);
        }
        public void UnEquip(IEquipContext<EquipmentTypes> context)
        {
            context.UnEquip(this);
        }
    }
}