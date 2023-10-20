using RPG.Core;
using System;

namespace InventorySystem
{
    public interface IUI_InventoryDisplay
    {
        string ID { get; }
        ISlotData AcceptDrop(string i,ISlotData input,UI_GhostIcon context);
    }

    public interface IUI_ChangeableInventoryDisplay<TInv> : IUI_InventoryDisplay
    {
        bool IsLinked { get; }
        void ChangeInventory(TInv inv);
    }
}
