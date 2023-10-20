namespace InventorySystem
{
    public interface IUI_InventoryController<TInv, TIndex> where TInv : class, ISlotContainer<TIndex>
    {
        TInv Inventory { get; }
        public bool IsLinked { get; }
        public void OnClick(TIndex index);
        public ISlotData OnDrag(TIndex index);
        public ISlotData OnDrop(TIndex index,ISlotData input,out bool wasError);
        void AssignNewInventory(TInv inv);
    }
}
