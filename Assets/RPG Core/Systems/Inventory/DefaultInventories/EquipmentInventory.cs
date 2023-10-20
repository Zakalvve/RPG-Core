using Item;
using RPGCore.Item;
using System.Collections.Generic;

namespace InventorySystem
{
    public class EquipmentInventory : EquipmentSlotContainer<IEquipableItem<EquipmentTypes>,EquipmentTypes>
    {
        public EquipmentInventory(IEquipContext<EquipmentTypes> context,IEquipmentConfig config,Dictionary<string,E_InventorySlot> dataBindings) : base(context,config, dataBindings) { }
    }
}
