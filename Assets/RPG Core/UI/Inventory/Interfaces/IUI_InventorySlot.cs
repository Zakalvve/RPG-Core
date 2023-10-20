using UnityEngine.UIElements;

namespace InventorySystem
{
    public interface IUI_InventorySlot
    {
        void Update(ISlotData d);
        void SetVisibile(Visibility v);
        Image Icon { get; }
        TextElement Stacks { get; }
    }
}
