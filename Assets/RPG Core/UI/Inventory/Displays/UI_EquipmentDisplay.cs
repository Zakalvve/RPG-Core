using System.Collections.Generic;
using UnityEngine.UIElements;
using Zenject;

namespace InventorySystem
{
    public class UI_EquipmentDisplay : UI_BaseChangeableInventoryDisplay<IEquipmentInventory,string>
    {
        protected Dictionary<string,UI_BaseInventorySlot> _equipmentSlots;
        protected IEquipmentConfig _equipConfig;

        [Inject]
        public UI_EquipmentDisplay(UIDocument UI, string displayRoot,IUI_InventoryController<IEquipmentInventory, string> controller, TooltipController tt,UI_GhostIcon ghostIcon,IEquipmentConfig equipConfig) : base(UI,displayRoot,controller, tt,ghostIcon) 
        {
            _equipConfig = equipConfig;
        }

        protected override void InitialiseDisplay()
        {
            //loop through the enums in the enum provided find the root for each slot and attack it - then add to the hash map
            _equipmentSlots = new Dictionary<string,UI_BaseInventorySlot>();

            _displayRoot.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            string[] results = _equipConfig.SlotNames;

            for (int i = 0; i < results.Length; i++)
            {
                var slotType = results[i].ToLower();

                //was inventory slot
                var slot = _displayRoot.panel.visualTree.Q<UI_BaseInventorySlot>(slotType);
                if (slot == null) continue;
                slot.name = UI_InventoryManager.FormID(ID,results[i].ToString());
                slot.Icon.name = $"UISlot_{ID}_{i}";
                slot.Stacks.name = $"UISlotStackCount_{ID}_{i}";
                slot.RegisterCallback<DragStartEvent,string>(HandleDragStart,results[i]);
                _equipmentSlots[results[i]] = slot;
            }

            var WeaponSet1 = _displayRoot.panel.visualTree.Q<Button>("btn-set-0");
            var WeaponSet2 = _displayRoot.panel.visualTree.Q<Button>("btn-set-1");
            var WeaponSet3 = _displayRoot.panel.visualTree.Q<Button>("btn-set-2");
            var WeaponSet4 = _displayRoot.panel.visualTree.Q<Button>("btn-set-3");

            WeaponSet1.RegisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut,0);
            WeaponSet2.RegisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut,1);
            WeaponSet3.RegisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut,2);
            WeaponSet4.RegisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut,3);
        }

        private void OnChangeWeaponLoadOut(ClickEvent e, int loadout)
        {
            _controller.Inventory.ChangeLoadout("WEAPON",loadout);
        }

        protected override void DestroyDisplay()
        {
            _displayRoot.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);

            foreach (var keyvalue in _equipmentSlots)
            {
                keyvalue.Value.UnregisterCallback<DragStartEvent,string>(HandleDragStart);
            }

            var WeaponSet1 = _displayRoot.panel.visualTree.Q<Button>("btn-set-0");
            var WeaponSet2 = _displayRoot.panel.visualTree.Q<Button>("btn-set-1");
            var WeaponSet3 = _displayRoot.panel.visualTree.Q<Button>("btn-set-2");
            var WeaponSet4 = _displayRoot.panel.visualTree.Q<Button>("btn-set-3");

            WeaponSet1.UnregisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut);
            WeaponSet2.UnregisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut);
            WeaponSet3.UnregisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut);
            WeaponSet4.UnregisterCallback<ClickEvent,int>(OnChangeWeaponLoadOut);
        }

        public override void ChangeInventory(IEquipmentInventory inv)
        {
            if (IsLinked)
            {
                foreach (var kvp in _equipmentSlots)
                {
                    _controller.Inventory[kvp.Key].Unbind(kvp.Value.Update);
                    kvp.Value.UnregisterCallback<MouseEnterEvent,string>(HandleMouseEnter);
                    kvp.Value.UnregisterCallback<MouseLeaveEvent>(HandleMouseLeave);
                }
            }
            _controller.AssignNewInventory(inv);
            foreach (var kvp in _equipmentSlots)
            {
                _controller.Inventory[kvp.Key].Bind(kvp.Value.Update);
                kvp.Value.RegisterCallback<MouseEnterEvent,string>(HandleMouseEnter,kvp.Key);
                kvp.Value.RegisterCallback<MouseLeaveEvent>(HandleMouseLeave);
            }
        }
        protected override void HandleDragStart(DragStartEvent e,string index)
        {
            if (!IsLinked) return;
            HideTooltip();
            ISlotData data = _controller.OnDrag(index);
            _ghostIcon.StartDrag(data,(slotData) => AcceptDrop(index,slotData,_ghostIcon));
        }
        public override ISlotData AcceptDrop(string index,ISlotData input,UI_GhostIcon context)
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
        protected override string ConvertStringToTIndex(string index)
        {
            return index;
        }

        protected virtual void HandleMouseEnter(MouseEnterEvent e,string index)
        {
            if (_ghostIcon.IsDragging) return;
            ShowTooltip(index);
        }

        protected override void ShowTooltip(string index)
        {
            if (_tt == null) return;
            if (!_controller.Inventory[index].IsEmpty)
            {
                _tt.ShowTooltip(_equipmentSlots[index],_controller.Inventory[index].Item);
            }
        }

        protected virtual void HandleMouseLeave(MouseLeaveEvent e)
        {
            HideTooltip();
        }
    }
}
