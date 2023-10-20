namespace InventorySystem
{
    public interface IEquipmentSlotContainer<TEnum> : ISlotContainer<string>
    {
        string[] SlotNames { get; }
        ISlotData Equip(ISlotData item,TEnum targetSlot);
        ISlotData Equip(ISlotData item,string targetSlot);
        ISlotData UnEquip(TEnum targetSlot);
        ISlotData UnEquip(string targetSlot);
    }
}
