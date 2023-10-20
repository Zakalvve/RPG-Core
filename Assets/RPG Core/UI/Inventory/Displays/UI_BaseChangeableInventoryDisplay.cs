using UnityEngine.UIElements;
using UnityEngine;
using System;
using Zenject;

namespace InventorySystem
{
    [Serializable]
    public abstract class UI_BaseChangeableInventoryDisplay<TInv, TIndex> : UI_BaseInventoryDisplay, IUI_ChangeableInventoryDisplay<TInv>
        where TInv : class, ISlotContainer<TIndex>
    {
        protected VisualElement _displayRoot;
        protected IUI_InventoryController<TInv,TIndex> _controller;
        protected TooltipController _tt;
        protected UI_GhostIcon _ghostIcon;

        [Inject]
        public UI_BaseChangeableInventoryDisplay(UIDocument UI, string displayRoot,IUI_InventoryController<TInv,TIndex> controller, TooltipController tt, UI_GhostIcon ghostIcon) : base()
        {
            _displayRoot = UI.rootVisualElement.Q<VisualElement>(displayRoot);
            _controller = controller;
            _tt = tt;
            _ghostIcon = ghostIcon;
            InitialiseDisplay();
        }

        [SerializeField]
        public bool IsLinked => _controller.IsLinked;
        protected abstract void InitialiseDisplay();
        public abstract void ChangeInventory(TInv inv);
        protected abstract void HandleDragStart(DragStartEvent e,TIndex index);
        protected abstract TIndex ConvertStringToTIndex(string index);
        protected abstract void ShowTooltip(TIndex index);
        protected virtual void HideTooltip()
        {
            if (_tt == null) return;
            _tt.HideTooltip();
        }
        ~UI_BaseChangeableInventoryDisplay() { DestroyDisplay(); }
    }
}
