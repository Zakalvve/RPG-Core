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
            Container.Bind<IUI_ChangeableInventoryDisplay<Inventory>>().To<UI_InventoryDisplay<Inventory>>().AsTransient().WithArguments("inventory");
            Container.Bind<IUI_InventoryController<Inventory,int>>().To<UI_InventoryController<Inventory,IItem>>().AsTransient();

            //Stash Displays
            Container.Bind<IUI_ChangeableInventoryDisplay<FilterableInventory>>().To<UI_PartyStashDisplay>().AsTransient().WithArguments("party-inventory");
            Container.Bind<IUI_InventoryController<FilterableInventory,int>>().To<UI_InventoryController<FilterableInventory,IItem>>().AsTransient();

            //Equipment Displays
            Container.Bind<IUI_ChangeableInventoryDisplay<EquipmentInventory>>().To<UI_EquipmentDisplay>().AsTransient().WithArguments("equipment");
            Container.Bind<IUI_InventoryController<EquipmentInventory,string>>().To<UI_EquipmentController<EquipmentInventory,IEquipableItem<EquipmentTypes>>>().AsTransient();

            //Action Bar displays
            Container.Bind<IUI_ChangeableInventoryDisplay<ActionBar>>().To<UI_ActionBarDisplay>().AsTransient().WithArguments("action-bar-root");
            Container.Bind<IUI_InventoryController<ActionBar,int>>().To<UI_ActionBarController>().AsTransient();

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