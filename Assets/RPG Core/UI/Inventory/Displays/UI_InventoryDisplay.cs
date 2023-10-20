using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace InventorySystem
{
    public class UI_InventoryDisplay<TInv> : UI_BaseChangeableInventoryDisplay<TInv,int>
        where TInv : class, ISlotContainer<int>
    {
        protected List<UI_BaseInventorySlot> _slots;
        [Inject]
        public UI_InventoryDisplay(UIDocument UI, string displayRoot, IUI_InventoryController<TInv,int> controller,TooltipController tt,UI_GhostIcon ghostIcon) : base(UI, displayRoot,controller, tt,ghostIcon) { }
        //keep
        protected override void InitialiseDisplay()
        {
            _slots = _displayRoot.Query<UI_BaseInventorySlot>().ToList();

            for (int i = 0; i < _slots.Count; i++)
            {
                //All slots belonging to this display are tagged with its ID
                var slot = _slots[i];
                slot.name = UI_InventoryManager.FormID(ID,i.ToString());
                slot.Icon.name = $"UISlot_{ID}_{i}";
                slot.Stacks.name = $"UISlotStackCount_{ID}_{i}";
                slot.AddToClassList("draggable");
                slot.RegisterCallback<DragStartEvent,int>(HandleDragStart,i);
                _displayRoot.Add(slot);
            }
        }
        //keep
        protected override void DestroyDisplay()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].UnregisterCallback<DragStartEvent,int>(HandleDragStart);
            }
            DetatchInventory();
        }
        //keep
        public override void ChangeInventory(TInv inv)
        {
            DetatchInventory();
            _controller.AssignNewInventory(inv);
            AttachInventory();
        }
        protected virtual void DetatchInventory()
        {
            if (IsLinked)
            {
                for (int i = 0; i < _slots.Count; i++)
                {
                    _controller.Inventory[i].Unbind(_slots[i].Update);
                    _slots[i].UnregisterCallback<MouseEnterEvent,int>(HandleMouseEnter);
                    _slots[i].UnregisterCallback<MouseLeaveEvent>(HandleMouseLeave);
                }
            }
        }
        protected virtual void AttachInventory()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _controller.Inventory[i].Bind(_slots[i].Update);
                _slots[i].RegisterCallback<MouseEnterEvent,int>(HandleMouseEnter,i);
                _slots[i].RegisterCallback<MouseLeaveEvent>(HandleMouseLeave);
            }
        }
        protected virtual void HandleMouseEnter(MouseEnterEvent e, int index)
        {
            if (_ghostIcon.IsDragging) return;
            ShowTooltip(index);
        }
        protected virtual void HandleMouseLeave(MouseLeaveEvent e)
        {
            HideTooltip();
        }
        //move
        protected override void ShowTooltip(int index)
        {
            if (_tt == null) return;
            if (!_controller.Inventory[index].IsEmpty)
            {
                _tt.ShowTooltip(_slots[index],_controller.Inventory[index].Item);
            }
        }
        //move
        protected override void HandleDragStart(DragStartEvent e,int index)
        {
            if (!IsLinked) return;
            HideTooltip();
            ISlotData data = _controller.OnDrag(index);
            _ghostIcon.StartDrag(data,(slotData) => AcceptDrop(index,slotData,_ghostIcon));
        }
        //move
        public override ISlotData AcceptDrop(string index,ISlotData input,UI_GhostIcon context)
        {
            try
            {
                int parsedIndex = ConvertStringToTIndex(index);
                return AcceptDrop(parsedIndex,input,context);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(InvalidCastException)) Debug.LogError("The string index cannot be used as an index in this display as it cannot be cast to type int");
                else Debug.LogError(e.Message);
                return input;
            }
        }
        //move
        protected virtual ISlotData AcceptDrop(int index,ISlotData input,UI_GhostIcon context)
        {
            var output = _controller.OnDrop(index,input,out bool wasError);
            if (!wasError)
            {
                if (!output.IsEmpty)
                {
                    context.StartDrag(output,(slotData) => AcceptDrop(index,slotData,context));
                }
            }
            else
            {
                if (!output.IsEmpty)
                {
                    context.StartDrag(output);
                }
            }
            return output;
        }
        protected override int ConvertStringToTIndex(string index)
        {
            return Convert.ToInt32(index);
        }
    }
}