using RPGCore.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    //Provides a mapping between different inventory slots and the equipment types than can be put in them
    public class EquipmentConfig : IEquipmentConfig
    {
        public Dictionary<string,EquipmentTypes> SlotBindings { get => _slotBindings; }
        Dictionary<string,EquipmentTypes> _slotBindings = new Dictionary<string,EquipmentTypes>();
        public string[] SlotNames => _slotBindings.Keys.ToArray();
        public Dictionary<string,EquipmentLoadout> Loadouts => _loadouts;
        private Dictionary<string,EquipmentLoadout> _loadouts = new Dictionary<string, EquipmentLoadout>();
        public void Add(string slotName,EquipmentTypes acceptType)
        {
            _slotBindings.Add(slotName,acceptType);
        }
        public void AddLoadout(string loadOutName, IEnumerable<KeyValuePair<string, EquipmentTypes>> loadout, int numberOfProfiles)
        {
            //create loadout
            if (numberOfProfiles > 1) _loadouts.Add(loadOutName,new EquipmentLoadout() { name = loadOutName,selectedProfile = 0, totalLoadouts = numberOfProfiles, loadoutBindings = loadout.ToList() });

            //index and add slot bindings for loadout
            for (int i = 0; i < numberOfProfiles; i++)
            {
                foreach (var kvp in loadout)
                {
                    if (numberOfProfiles == 1)
                    {
                        Add(kvp.Key, kvp.Value);
                    }
                    else
                    {
                        Add($"{loadOutName}-{kvp.Key}-{i}",kvp.Value);
                    }
                }
            }
        }
        public static bool SplitLoadoutKey(string key,out (string loadoutName, string slot, int index) output)
        {
            output = ("", "", -1);
            var split = key.Split('-');
            if (split.Length <= 1)
            {
                return false;
            }
            try
            {
                int i = Convert.ToInt32(split[2]);
                output = (split[0], split[1], i);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
        public class EquipmentLoadout
        {
            public string name;
            public int selectedProfile;
            public int totalLoadouts;
            public List<KeyValuePair<string,EquipmentTypes>> loadoutBindings;
        }
    }
}
