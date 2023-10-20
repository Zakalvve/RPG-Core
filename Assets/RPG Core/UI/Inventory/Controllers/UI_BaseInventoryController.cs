using UnityEngine;
using Zenject;

namespace InventorySystem
{
    public abstract class UI_BaseInventoryController<TInv, TIndex> : IUI_InventoryController<TInv,TIndex> where TInv : class, ISlotContainer<TIndex>
    {
        [Inject]
        protected ISystemMessenger _messenger;
        public UI_BaseInventoryController() { Inventory = null; }
        public UI_BaseInventoryController(TInv inventory)
        {
            Inventory = inventory;
        }
        public bool IsLinked => Inventory != null;
        public TInv Inventory { get; private set; }
        //by default no functionality is supported - derrived class can override and implement different functionality as required
        public virtual void OnClick(TIndex index) { Debug.LogWarning("You can't use that."); }
        public virtual ISlotData OnDrag(TIndex index)
        {
            Debug.LogWarning("You can't move that.");
            _messenger.ShowMessage("You can't move that.",Color.red);
            return TemporarySlot.Empty;
        }
        public virtual ISlotData OnDrop(TIndex index,ISlotData input,out bool wasError)
        {
            _messenger.ShowMessage("You can't put that there.",Color.red);
            Debug.LogWarning("You can't put that there.");
            wasError = true;
            return input;
        }

        public void AssignNewInventory(TInv inv)
        {
            Inventory = inv;
        }
    }
}