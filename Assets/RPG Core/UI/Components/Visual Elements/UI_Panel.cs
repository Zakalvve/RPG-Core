using UnityEngine.UIElements;
using UnityEngine;
using InventorySystem;

namespace RPG.Core.UI
{
    //a panel is a single self contained UI element such as an action bar, character sheet, combat tracker etc
    //panels support show/hide functionality

    public class UI_Panel : VisualElement, IHideable
    {
        private VisualElement dragHandle;
        public new class UxmlFactory : UxmlFactory<UI_Panel> { }
        public bool IsHidden { get; private set; }
        public void Hide()
        {
            style.display = DisplayStyle.None;
            IsHidden = true;
        }
        public void Show()
        {
            style.display = DisplayStyle.Flex;
            IsHidden = false;
        }
        public void InitializePanel()
        {
            //if a panel has a tool bar, the panel is draggable
            dragHandle = this.Q<VisualElement>("title-bar");
            if (dragHandle != null)
            {
                //add drag support
                dragHandle.AddToClassList("draggable");
                dragHandle.RegisterCallback<DragStartEvent>(DragBegin);
                dragHandle.RegisterCallback<DragMoveEvent>(MouseMove);
            }
        }

        //drag support
        private Vector2 _startPos;
        private Vector2 _startWindowPosition;

        void DragBegin(DragStartEvent e)
        {
            if (IsHidden) return;
            _startPos = e.localMousePosition;
            _startWindowPosition = new Vector2(layout.xMin,layout.yMin);
        }
        void MouseMove(DragMoveEvent e)
        {
            if (IsHidden)
            {
                return;
            }
            var delta = e.localMousePosition - _startPos;
            float topPosX = _startWindowPosition.x + delta.x;
            float topPosY = _startWindowPosition.y + delta.y;

            style.left = topPosX;
            style.top = topPosY;
        }

        ~UI_Panel()
        {
            if (dragHandle != null)
            {
                dragHandle.UnregisterCallback<DragStartEvent>(DragBegin);
                dragHandle.UnregisterCallback<DragMoveEvent>(MouseMove);
            }
        }
    }
}