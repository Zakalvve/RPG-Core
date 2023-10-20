using RPGCore.Item;
using System.Collections.Generic;
using static InventorySystem.EquipmentConfig;

namespace InventorySystem
{
    public interface IEquipmentConfig
    {
        Dictionary<string,EquipmentTypes> SlotBindings { get; }
        string[] SlotNames { get; }
        Dictionary<string,EquipmentLoadout> Loadouts { get; }
    }
}