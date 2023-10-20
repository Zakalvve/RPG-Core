using InventorySystem;
using Item;
using UnityEngine;
using BansheeGz.BGDatabase;
using RPG.Core.Character.Attributes;
using Zenject;
using RPGCore.Item;
using System;

namespace RPG.Core.Character
{
    public class PartyCharacter : M_Character, IPartyCharacter, IActionContext, IEquipContext<EquipmentTypes>, BGAddonSaveLoad.BeforeSaveReciever
    {
        #region IPartyCharacter
        public string ID => Entity.Id.ToString();
        public string Name => Entity.Name;
        public Sprite Portrait => f_portrait;

        //stats
        public StatBlock Stats { get; private set; }

        //inventory
        public Inventory Bags { get; private set; }
        public ActionBar ActionBar { get; private set; }
        public EquipmentInventory Equipment { get; private set; }
        #endregion

        #region Effects
        //to implement
        #endregion

        #region Dependencies
        private ISystemMessenger _messenger;
        #endregion

        #region Initialize
        [Inject]
        public void Initialize(StatBlock statBlock,Inventory inventory,ActionBar actionBar,EquipmentInventory equipmentInventory,ISystemMessenger messenger)
        {
            Stats = statBlock;
            Bags = inventory;
            ActionBar = actionBar;
            Equipment = equipmentInventory;

            _messenger = messenger;
            transform.position = f_position;
            gameObject.name = Entity.Name;
        }
        #endregion

        #region IActionContext
        public void Use(IActionItem action)
        {
            _messenger.ShowMessage($"{Name} Used: {action.f_name}",Color.yellow);
            Debug.Log($"Used: {action.f_name}");
        }
        #endregion

        #region IEquiContext
        public void Equip(IEquipableItem<EquipmentTypes> item)
        {
            _messenger.ShowMessage($"Equipped: {item.f_name}",Color.green);
            Debug.Log($"Equipped: {item.f_name}");
        }
        public void UnEquip(IEquipableItem<EquipmentTypes> item)
        {
            _messenger.ShowMessage($"Un-equipped: {item.f_name}",Color.magenta);
            Debug.Log($"Un-equipped: {item.f_name}");
        }
        #endregion

        #region BeforeSaveReciever
        public void OnBeforeSave()
        {
            if (!gameObject.activeInHierarchy) return;

            if (!gameObject.scene.IsValid()) return;

            f_position = transform.position;
        }
        #endregion

        [ContextMenu("UpdatePosition")]
        void UpdateDbPosition()
        {
            f_position = transform.position;
        }
    }
}
