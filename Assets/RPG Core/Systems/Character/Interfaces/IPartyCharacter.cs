using Assets.RPG_Core.Core;
using InventorySystem;
using RPG.Core.Character.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG.Core.Character
{
    public interface IPartyCharacter : ICharacter
    {
        Inventory Bags { get; }
        EquipmentInventory Equipment { get; }
        ActionBar ActionBar { get; }
    }

    public interface ICharacter : IIDable
    {
        string Name { get; }
        Sprite Portrait { get; }
        StatBlock Stats { get; }
    }
}
