using InventorySystem;
using Item;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using RPGCore.Item;
using System.Collections.Generic;

namespace RPG.Core.Installers
{
    public class UIInstaller : MonoInstaller
    {
        public GameObject UIRoot;
        public override void InstallBindings()
        {
            //UI Root
            Container.Bind<UIDocument>().FromComponentOn(UIRoot).AsSingle();

            //UI Components
            Container.Bind<UI_CharacterSheet>().FromComponentInHierarchy().AsSingle();
            Container.Bind<UI_ActionBar>().FromComponentInHierarchy().AsSingle();

            //UI Services
            Container.Bind<UI_GhostIcon>().AsSingle();
            Container.Bind<MouseDragController>().AsSingle();
            Container.Bind<TooltipController>().AsSingle();

            //Inventory Displays
            Container.Bind<IUI_ChangeableInventoryDisplay<IInventory>>().To<UI_InventoryDisplay<IInventory>>().AsTransient().WithArguments("inventory");
            Container.Bind<IUI_InventoryController<IInventory,int>>().To<UI_InventoryController<IInventory,IItem>>().AsTransient();

            //Stash Displays
            Container.Bind<IUI_ChangeableInventoryDisplay<IPartyStash>>().To<UI_PartyStashDisplay>().AsTransient().WithArguments("party-inventory");
            Container.Bind<IUI_InventoryController<IPartyStash,int>>().To<UI_InventoryController<IPartyStash,IItem>>().AsTransient();

            //Equipment Displays
            Container.Bind<IUI_ChangeableInventoryDisplay<IEquipmentInventory>>().To<UI_EquipmentDisplay>().AsTransient().WithArguments("equipment");
            Container.Bind<IUI_InventoryController<IEquipmentInventory,string>>().To<UI_EquipmentController<IEquipmentInventory,IEquipableItem<EquipmentTypes>>>().AsTransient();

            //Action Bar displays
            Container.Bind<IUI_ChangeableInventoryDisplay<IActionBar>>().To<UI_ActionBarDisplay>().AsTransient().WithArguments("action-bar-root");
            Container.Bind<IUI_InventoryController<IActionBar,int>>().To<UI_ActionBarController>().AsTransient();

            List<E_InventorySlot> ghostSlot = new List<E_InventorySlot>();
            var newSlot = E_InventorySlot.NewEntity();
            newSlot.f_name = "UI_GhostSlot";
            ghostSlot.Add(newSlot);

            //UI Other
            Container.Bind<Inventory>().AsSingle().WithArguments(ghostSlot);
            Container.Bind<UI_BaseInventorySlot>().FromInstance(new UI_BaseInventorySlot("ghost-icon")).AsSingle();
        }
    }
}