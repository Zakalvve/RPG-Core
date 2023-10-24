using UnityEngine.UIElements;
using Zenject;

namespace InventorySystem
{
    public class UI_ActionBarDisplay : UI_InventoryDisplay<IActionBar>
    {
        [Inject]
        public UI_ActionBarDisplay(UIDocument UI, string displayRoot, IUI_InventoryController<IActionBar, int> controller, TooltipController tt,UI_GhostIcon ghostIcon) : base(UI,displayRoot,controller, tt,ghostIcon) { }
        protected override void InitialiseDisplay()
        {
            base.InitialiseDisplay();
            for (int i = 0; i < _slots.Count; i++)
            {
                //All slots belonging to this display are tagged with its ID
                var slot = _slots[i];
                slot.Icon.name = $"UIActiveSlot_{ID}_{i}";
                slot.Stacks.name = $"UIActiveSlotStackCount_{ID}_{i}";
                slot.AddToClassList("dragable-action");
                slot.RegisterCallback<DragClickEvent,int>(HandleClick,i);
            }
        }

        protected override void DestroyDisplay()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].UnregisterCallback<DragClickEvent,int>(HandleClick);
            }
            base.DestroyDisplay();
        }
        protected void HandleClick(DragClickEvent e,int index)
        {
            if (!IsLinked) return;
            _controller.OnClick(index);
        }
    }
}