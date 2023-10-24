using Assets.RPG_Core.Core;
using InventorySystem;
using RPG.Core.Character.Attributes;
using UnityEngine;

namespace RPG.Core.Character
{
    public interface ICharacterData : IIDable
    {
        string Name { get; }
        Sprite Portrait { get; }
        AttributeSet Stats { get; }
        IInventory Bags { get; }
        IEquipmentInventory Equipment { get; }
        IActionBar ActionBar { get; }
    }
}
