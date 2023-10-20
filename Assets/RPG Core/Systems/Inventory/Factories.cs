using BansheeGz.BGDatabase;
using Item;
using JetBrains.Annotations;
using RPG.Core.Character;
using RPG.Core.Character.Attributes;
using RPG.Core.SerializableDictionary;
using RPGCore.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InventorySystem
{
    public class Factories
    {
        public class PartyMemberFactory : IFactory<List<PartyCharacterController>>
        {
            [Inject]
            readonly DiContainer container;
            public List<PartyCharacterController> Create()
            {
                var dbParty = E_Party.GetEntity(0);
                var dbMembers = dbParty.f_Member;

                List<PartyCharacterController> sceneMembers = new List<PartyCharacterController>();

                foreach (var member in dbMembers)
                {
                    var partyCharacterPrefab = member.f_member.f_prefab;
                    var sub = container.CreateSubContainer();
                    sub.Bind<PartyCharacterController>().FromComponentInNewPrefab(partyCharacterPrefab).AsSingle();
                    var controller = sub.Resolve<PartyCharacterController>();
                    sceneMembers.Add(controller);
                }
                return sceneMembers;
            }
        }
        public class InventoryFactory : IFactory<Inventory>
        {
            [Inject]
            PartyCharacter character;
            const int capacity = 20;

            public Inventory Create()
            {
                return CreateInventoryFromCharacter(character,capacity);
            }
            private static Inventory CreateInventoryFromCharacter(PartyCharacter character,int expectedCapacity)
            {
                var bag = ((E_Inventory)character.f_bags);

                if (bag == null)
                {
                    bag = E_Inventory.NewEntity();
                    bag.f_name = $"{character.f_name}_Inventory";
                }
                character.f_bags = bag;
                var dataSlots = bag.f_slots;

                if (!ValidateSlots(dataSlots,expectedCapacity))
                {
                    dataSlots = CreateDataSlots(character.f_name,"Inventory",dataSlots,expectedCapacity);
                }
                bag.f_slots = dataSlots;
                return new Inventory(dataSlots);
            }
        }
        public class ActionBarFactory : IFactory<ActionBar>
        {
            [Inject]
            PartyCharacter character;
            const int capacity = 12;
            public ActionBar Create()
            {
                return CreateActionBarFromCharacter(character,capacity);
            }
            private static ActionBar CreateActionBarFromCharacter(PartyCharacter character,int expectedCapacity)
            {
                var actionBar = ((E_ActionBar)character.f_actionBar);

                if (actionBar == null)
                {
                    actionBar = E_ActionBar.NewEntity();
                    actionBar.f_name = $"{character.f_name}_ActionBar";
                }
                character.f_actionBar = actionBar;
                var dataSlots = actionBar.f_slots;

                if (!ValidateSlots(dataSlots,expectedCapacity))
                {
                    dataSlots = CreateDataSlots(character.f_name,"Action",dataSlots,expectedCapacity);
                }
                actionBar.f_slots = dataSlots;
                return new ActionBar(dataSlots,character);
            }
        }
        public class EquipmentInventoryFactory : IFactory<EquipmentInventory>
        {
            [Inject]
            PartyCharacter character;
            [Inject]
            IEquipmentConfig config;

            public EquipmentInventory Create()
            {
                return CreateEquipmentInventoryFromCharacter(character, config);
            }
            private static EquipmentInventory CreateEquipmentInventoryFromCharacter(PartyCharacter character,IEquipmentConfig config)
            {
                Dictionary<string,E_InventorySlot> dataBindings = new Dictionary<string,E_InventorySlot>();

                var equipment = ((E_EquipmentInventory)character.f_equipment);

                if (equipment == null)
                {
                    equipment = E_EquipmentInventory.NewEntity();
                    equipment.f_name = $"{character.f_name}_Equipment";
                    character.f_equipment = equipment;
                }

                dataBindings = ExtractEquipmentDataBindings(character,config.SlotBindings);

                return new EquipmentInventory(character,config,dataBindings);
            }
            public static Dictionary<string,E_InventorySlot> ExtractEquipmentDataBindings(PartyCharacter character,Dictionary<string,EquipmentTypes> input)
            {
                var output = new Dictionary<string,E_InventorySlot>();
                var equipment = ((E_EquipmentInventory)character.f_equipment);

                if (equipment == null)
                {
                    equipment = E_EquipmentInventory.NewEntity();
                    equipment.f_name = $"{character.f_name}_Equipment";
                    character.f_equipment = equipment;
                }

                var currentSlots = equipment.f_slots ?? new List<E_EquipmentSlot>();

                foreach (var kvp in input)
                {
                    var invSlot = currentSlots.Find(slot => slot.f_name == kvp.Key);
                    if (invSlot != null)
                    {
                        output.Add(kvp.Key,invSlot.f_slot);
                    }
                    else
                    {
                        invSlot = E_EquipmentSlot.NewEntity();
                        invSlot.f_name = kvp.Key;
                        var newEquipSlot = E_InventorySlot.NewEntity();
                        newEquipSlot.f_name = $"{character.f_name}_EquipmentSlot{kvp.Key}";
                        invSlot.f_slot = newEquipSlot;
                        invSlot.f_ValidSlotType = kvp.Value;
                        currentSlots.Add(invSlot);
                        output.Add(kvp.Key,invSlot.f_slot);
                    }
                }

                equipment.f_slots = currentSlots;

                return output;
            }
        }
        public class EquipConfigFactory : IFactory<IEquipmentConfig>
        {
            public IEquipmentConfig Create()
            {
                return CreateDefaultEquipConfig();
            }
            public static IEquipmentConfig CreateDefaultEquipConfig()
            {
                EquipmentConfig config = new EquipmentConfig();


                E_EquipmentInvLoadout.ForEachEntity(loadout =>
                {
                    var loadoutSlots = new List<KeyValuePair<string,EquipmentTypes>>();
                    foreach (var equipmentType in loadout.f_Profile.f_ProfileSlots)
                    {
                        loadoutSlots.Add(new KeyValuePair<string,EquipmentTypes>(equipmentType.ToString(),equipmentType));
                    }

                    config.AddLoadout(loadout.f_name,loadoutSlots,loadout.f_numberProfiles);
                });

                return config;
            }
        }
        public class PartyStashFactory : IFactory<FilterableInventory>
        {
            const int capacity = 63;
            public FilterableInventory Create()
            {
                return CreatePartyStash(capacity);
            }
            private static FilterableInventory CreatePartyStash(int expectedCapacity)
            {
                var party = E_Party.GetEntity(0);

                if (party.f_Stash == null)
                {
                    party.f_Stash = E_Inventory.NewEntity();
                    party.f_Stash.f_name = $"Party_Stash";
                }

                var dataSlots = party.f_Stash.f_slots;

                if (!ValidateSlots(dataSlots,expectedCapacity))
                {
                    dataSlots = CreateDataSlots("Party","Stash",dataSlots,expectedCapacity);
                }

                party.f_Stash.f_slots = dataSlots;

                return new FilterableInventory(dataSlots);
            }
        }
        public class StatBlockFactory : IFactory<StatBlock>
        {
            [Inject]
            PartyCharacter character;
            public StatBlock Create()
            {
                return StatBlock.StatBlockFromCharacter(((E_Character)character.Entity));
            }
        }
        //helper methods
        public static TInstance CreateItem<TInstance>(E_v_item data) where TInstance : class, IItem
        {
            if (data is E_Action)
            {
                return (TInstance)(IItem)(new ActionItem((E_Action)data));
            }
            else if (data is E_Equipment)
            {
                return (TInstance)(IItem)(new Equipment((E_Equipment)data));
            }
            else if (data is E_Consumable)
            {
                return (TInstance)(IItem)(new Consumable((E_Consumable)data));
            }

            return (TInstance)(IItem)(new BaseItem<E_Item>((E_Item)data));
        }
        private static bool ValidateSlots(List<E_InventorySlot> slots,int expectedCapacity)
        {
            return slots?.Count == expectedCapacity;
        }
        private static List<E_InventorySlot> CreateDataSlots(string controllingEntityName, string inventoryType, List<E_InventorySlot> input,int expectedCapacity)
        {
            if (input != null && input.Count > expectedCapacity) throw new Exception("Data integrity error");
            if (ValidateSlots(input,expectedCapacity)) return input;
            var output = input ?? new List<E_InventorySlot>();
            for (int i = 0, j = output.Count; i < expectedCapacity - j; i++)
            {
                var newSlot = E_InventorySlot.NewEntity();
                newSlot.f_name = $"{controllingEntityName}_{inventoryType}Slot{i}";
                output.Add(newSlot);
            }
            return output;
        }
    }
}