using Assets.RPG_Core.Core;
using RPG.Core;

namespace InventorySystem
{
    public abstract class UI_BaseInventoryDisplay : InstanceIndexer<UI_BaseInventoryDisplay>, IUI_InventoryDisplay, IIDable
    {
        public abstract ISlotData AcceptDrop(string index,ISlotData input,UI_GhostIcon context);
        protected abstract void DestroyDisplay();
    }
}
