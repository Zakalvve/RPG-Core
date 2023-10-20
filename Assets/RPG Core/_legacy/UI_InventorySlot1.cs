//using InventorySystem;
//using UnityEngine.UIElements;

//public class UI_InventorySlot<TIndex> : UI_BaseInventorySlot, IUI_InventorySlot<TIndex>
//{
//    public TIndex Index { get; private set; }

//    public UI_InventorySlot(TIndex index, string displayId,string containerClassName) : base(containerClassName)
//    {
//        Index = index;
//        name = UI_InventoryManager.FormID(displayId,index.ToString());
//        AddToClassList("draggable");
//        _icon.name = $"UISlot_{displayId}_{index}";
//        _stacks.name = $"UISlotStackCount_{displayId}_{index}";
//    }
//    public UI_InventorySlot(TIndex index,string displayId) : base()  
//    {
//        Index = index;
//        name = UI_InventoryManager.FormID(displayId,index.ToString());
//        AddToClassList("draggable");
//        _icon.name = $"UISlot_{displayId}_{index}";
//        _stacks.name = $"UISlotStackCount_{displayId}_{index}";
//    }
//}
