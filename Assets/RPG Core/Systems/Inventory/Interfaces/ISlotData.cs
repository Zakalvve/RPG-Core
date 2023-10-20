using Item;

namespace InventorySystem
{
    public interface ISlotData : IBindableObject<ISlotData>
    {
#nullable enable
        IItem? Item { get; }
#nullable disable
        int StackSize { get; }
        bool IsEmpty { get; }
    }
}