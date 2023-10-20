using RPG.Core.ExtensionMethods;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace InventorySystem
{
    public class UI_GhostIcon
    {
        private UI_BaseInventorySlot _icon;
        private Inventory _slot;
        private MouseDragController _controller;
        private VisualElement _root;
        public bool IsDragging => _controller.IsDragging;
        public UI_GhostIcon(MouseDragController controller, UIDocument doc, UI_BaseInventorySlot icon, Inventory slot)
        {
            _controller = controller;
            _root = doc.rootVisualElement;
            _icon = icon;
            _slot = slot;

            _icon.style.height = 0;
            _icon.style.width = 75;
            _icon.style.paddingTop = 75;
            _icon.style.top = 0;
            _icon.style.left = 0;

            _icon.OnAllChildren((e) =>
            {
                e.pickingMode = PickingMode.Ignore;
                e.style.visibility = Visibility.Visible;
            });

            _root.RegisterCallback<DragMoveEvent>(GhostMove);
            _root.RegisterCallback<DragEndEvent>(GhostDrop);
            _root.Add(_icon);

            _icon.Update(_slot.Eject(0));
        }

        private static Func<ISlotData> _onReturn; //Called when a drop is unsucessful and the slot data must be returned to its original slot
        public void StartDrag(ISlotData data,Func<ISlotData,ISlotData> onReturn) => GhostDrag(data,onReturn);
        public void StartDrag(ISlotData data) => GhostDrag(data,_onReturn);
        private void GhostDrag(ISlotData data,Func<ISlotData,ISlotData> onReturn)
        {
            if (data.IsEmpty) return;
            GhostDrag(data,() => { return onReturn(_slot.Eject(0)); });
        }
        private void GhostDrag(ISlotData data,Func<ISlotData> onReturn)
        {
            if (_slot[0].IsEmpty && !data.IsEmpty)
            {
                _slot.Insert(data);

                _icon.OnAllChildren((e) => e.style.visibility = Visibility.Visible);
                _icon.Update(data);

                _onReturn = onReturn;
            }
            _icon.style.top = _controller.DragMousePosition.y - _icon.layout.height / 2;
            _icon.style.left = _controller.DragMousePosition.x - _icon.layout.width / 2;

        }
        private void GhostMove(DragMoveEvent e)
        {
            _icon.style.top = e.mousePosition.y - _icon.layout.height / 2;
            _icon.style.left = e.mousePosition.x - _icon.layout.width / 2;
        }
        private void GhostDrop(DragEndEvent e)
        {
            if (_slot.IsEmpty)
            {
                e.resolver.Resolve(); //drop was successful since there is nothign to drop
                return;
            }
            ISlotData output;
            VisualElement slot;
            if (!_controller.FindDragableElement(e.mousePosition,out slot))
            {
                output = _onReturn();
                if (!output.IsEmpty)
                {
                    GhostDrag(output,_onReturn);
                    e.resolver.Reject();
                    return;
                }
                else
                {
                    _icon.OnAllChildren((e) => e.style.visibility = Visibility.Hidden);
                    e.resolver.Resolve();
                    return;
                }
            }
            else
            {
                (string displayId, string slotIndex) = UI_InventoryManager.SplitID(slot.name);
                if (!UI_BaseInventoryDisplay.Instances.ContainsKey(displayId)) throw new KeyNotFoundException($"An inventory display with the key {displayId} could not be found.");
                output = UI_BaseInventoryDisplay.Instances[displayId].AcceptDrop(slotIndex,_slot.Eject(0),this);
                if (output.IsEmpty)
                {
                    _icon.OnAllChildren((e) => e.style.visibility = Visibility.Hidden);
                    e.resolver.Resolve();
                    return;
                }
                e.resolver.Reject();
                return;
            }
        }

        ~UI_GhostIcon()
        {
            _root.UnregisterCallback<DragMoveEvent>(GhostMove);
            _root.UnregisterCallback<DragEndEvent>(GhostDrop);
        }
    }
}
