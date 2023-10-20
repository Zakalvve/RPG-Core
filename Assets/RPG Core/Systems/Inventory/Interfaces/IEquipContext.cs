using Item;
using System;

namespace InventorySystem
{
    //A context within which an item is equipped/unequipped
    public interface IEquipContext<TEnum> where TEnum : struct, Enum
    {
        public void Equip(IEquipableItem<TEnum> item);
        public void UnEquip(IEquipableItem<TEnum> item);
    }
}