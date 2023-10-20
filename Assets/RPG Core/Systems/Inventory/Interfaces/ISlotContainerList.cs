using Item;

namespace InventorySystem
{
    public interface ISlotContainerList<TItem> : ISlotContainer<int> where TItem : class, IItem
    {
        bool IsEmpty { get; }
        bool IsFull { get; }
        int Capacity { get; }
        int Count { get; }
        ISlotData Insert(ISlotData slot);
        ISlotData Insert(ISlotData slot,int at);
        ISlotData Eject(int from,int stacksToRemove = int.MaxValue);
        int IndexOfFirst(TItem item);
    }
}