using InventorySystem;
using System;

namespace Item
{
    public interface IEquipableItem<TEnum> : IItem where TEnum : struct, Enum
    {
        TEnum f_ValidEquipmentSlot { get; }
        void Equip(IEquipContext<TEnum> context);
        void UnEquip(IEquipContext<TEnum> context);
    }
}