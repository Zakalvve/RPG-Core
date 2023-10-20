namespace InventorySystem
{
    public interface IActionSlotContainer<TIndex>
    {
        public void Execute(TIndex index);
    }
}
