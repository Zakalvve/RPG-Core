namespace InventorySystem
{
    public interface ISlotContainer<TIndex>
    {
        public ISlotData this[TIndex index] { get; }
    }
}
