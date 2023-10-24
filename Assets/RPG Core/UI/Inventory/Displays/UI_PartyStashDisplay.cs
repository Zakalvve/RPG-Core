using Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

namespace InventorySystem
{
    public class UI_PartyStashDisplay : UI_InventoryDisplay<IPartyStash>
    {
        [Inject]
        public UI_PartyStashDisplay(UIDocument UI, string displayRoot,IUI_InventoryController<IPartyStash,int> controller, TooltipController tt,UI_GhostIcon ghostIcon) : base(UI,displayRoot,controller, tt,ghostIcon) { }
        protected override void InitialiseDisplay()
        {
            base.InitialiseDisplay();
            _displayRoot.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }
        public void OnAttachToPanel(AttachToPanelEvent e)
        {
            //get control elements
            var filter = _displayRoot.panel.visualTree.Q<TextField>("txt-party-stash-search");
            var filterByArmour = _displayRoot.panel.visualTree.Q<Button>("btn-byarmour");
            var filterByWeapon = _displayRoot.panel.visualTree.Q<Button>("btn-byweapon");
            var filterByConsumable = _displayRoot.panel.visualTree.Q<Button>("btn-byconsumable");
            var sort = _displayRoot.panel.visualTree.Q<Button>("btn-sort");

            //register callbacks
            filter.RegisterCallback<ChangeEvent<string>>(OnFilter);
            filter.RegisterCallback<FocusInEvent>(OnFocusIn);
            filter.RegisterCallback<FocusOutEvent>(OnFocusOut);
            filterByArmour.RegisterCallback<ClickEvent, string>(OnFilterByType, "armour");
            filterByWeapon.RegisterCallback<ClickEvent, string>(OnFilterByType, "weapon");
            filterByConsumable.RegisterCallback<ClickEvent, string>(OnFilterByType, "consumable");
            sort.RegisterCallback<ClickEvent>(OnSort);
        }
        public void OnFilter(ChangeEvent<string> e)
        {
            _controller.Inventory.FilterByName(e.newValue);
        }
        public void OnFocusIn(FocusInEvent e)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }
        public void OnFocusOut(FocusOutEvent e)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        public void OnFilterByType(ClickEvent e, string type)
        {
            _controller.Inventory.FilterByType(type);
        }
        public void OnSort(ClickEvent e)
        {
            _controller.Inventory.SortByType();
        }
        public void Unbind(IPartyStash inv)
        {
            if (IsLinked)
            {
                for (int i = 0; i < inv.FilteredInventory.Count; i++)
                {
                    _controller.Inventory[i].Unbind(_slots[i].Update);
                    _slots[i].Update(TemporarySlot.Empty);
                    _slots[i].UnregisterCallback<MouseEnterEvent,int>(HandleMouseEnter);
                    _slots[i].UnregisterCallback<MouseLeaveEvent>(HandleMouseLeave);
                }
            }
        }
        public void Bind(IPartyStash inv)
        {
            for (int i = 0; i < inv.FilteredInventory.Count; i++)
            {
                _controller.Inventory[i].Bind(_slots[i].Update);
                _slots[i].RegisterCallback<MouseEnterEvent,int>(HandleMouseEnter, i);
                _slots[i].RegisterCallback<MouseLeaveEvent>(HandleMouseLeave);
            }
        }
        public override void ChangeInventory(IPartyStash inv)
        {
            if (inv == _controller.Inventory) return;
            if (IsLinked)
            {
                _controller.Inventory.OnBeforeFilter -= Unbind;
                _controller.Inventory.OnAfterFilter -= Bind;
                Unbind(inv);

            }
            _controller.AssignNewInventory(inv);
            _controller.Inventory.OnBeforeFilter += Unbind;
            _controller.Inventory.OnAfterFilter += Bind;
            Bind(inv);
        }
        protected override void DestroyDisplay()
        {
            _displayRoot.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);

            var filter = _displayRoot.panel.visualTree.Q<TextField>("txt-party-stash-search");
            var filterByArmour = _displayRoot.panel.visualTree.Q<Button>("btn-byarmour");
            var filterByWeapon = _displayRoot.panel.visualTree.Q<Button>("btn-byweapon");
            var filterByConsumable = _displayRoot.panel.visualTree.Q<Button>("btn-byconsumable");
            var sort = _displayRoot.panel.visualTree.Q<Button>("btn-sort");

            filter.UnregisterCallback<ChangeEvent<string>>(OnFilter);
            filter.UnregisterCallback<FocusInEvent>(OnFocusIn);
            filter.UnregisterCallback<FocusOutEvent>(OnFocusOut);
            filterByArmour.UnregisterCallback<ClickEvent,string>(OnFilterByType);
            filterByWeapon.UnregisterCallback<ClickEvent,string>(OnFilterByType);
            filterByConsumable.UnregisterCallback<ClickEvent,string>(OnFilterByType);
            sort.UnregisterCallback<ClickEvent>(OnSort);

            base.DestroyDisplay();
        }
    }
}
