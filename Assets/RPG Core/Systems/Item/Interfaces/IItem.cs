using Assets.RPG_Core.Core;
using UnityEngine;

namespace Item
{
    public interface IItem : IIDable
    {
        E_v_item Item { get; }
        string f_name { get; }
        string f_Type { get; }
        int f_MaxStackSize { get; }
        string f_Description { get; }
        Sprite f_Icon { get; }
    }
}