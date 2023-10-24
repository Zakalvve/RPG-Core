using Item;
using RPGCore.Item;
using System.Collections.Generic;

namespace InventorySystem
{
    public class EquipmentInventory : EquipmentSlotContainer<IEquipableItem<EquipmentTypes>,EquipmentTypes>, IEquipmentInventory
    {
        public EquipmentInventory(IEquipContext<EquipmentTypes> context,IEquipmentConfig config,Dictionary<string,E_InventorySlot> dataBindings) : base(context,config, dataBindings) { }
    }

    public interface IEquipmentInventory : IEquipmentSlotContainer<EquipmentTypes>
    {
        void ChangeLoadout(string name,int index);
    }
}
