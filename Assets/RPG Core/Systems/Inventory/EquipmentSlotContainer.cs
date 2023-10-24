using Item;
using RPGCore.Item;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace InventorySystem
{
    public class EquipmentSlotContainer<TItem, TEnum> : IEquipmentSlotContainer<TEnum> where TItem : class, IEquipableItem<TEnum> where TEnum : struct, Enum
    {
        protected Dictionary<string,SlotContainer<TItem>.Slot> _equipmentSlots;
        protected IEquipContext<TEnum> _context;
        protected IEquipmentConfig _config;
        private Dictionary<string,E_InventorySlot> _dataBindings = new Dictionary<string,E_InventorySlot>();

        public EquipmentSlotContainer(IEquipContext<TEnum> context, IEquipmentConfig config,Dictionary<string,E_InventorySlot> dataBindings)
        {
            _context = context;
            _config = config;
            _dataBindings = dataBindings;

            _equipmentSlots = new Dictionary<string,SlotContainer<TItem>.Slot>(_config.SlotBindings.Count);

            foreach (var kvp in _dataBindings)
            {
                _equipmentSlots.Add(kvp.Key, new SlotContainer<TItem>.Slot(-1, kvp.Value));
            }
        }
        public string[] SlotNames => _config.SlotNames;
        public ISlotData this[string slot]
        {
            get
            {
                slot = slot.ToUpper();
                if (!_equipmentSlots.ContainsKey(slot)) throw new KeyNotFoundException("A slot with that key could not be found");
                return _equipmentSlots[slot].Peek();
            }
        }
        protected TEnum GetSlotTypeFromString(string targetSlot)
        {
            if (!Enum.TryParse(targetSlot,true,out TEnum type))
            {
                throw new ArgumentException("The given string cannot be converted into a valid value from the TEnum enumeration");
            }
            return type;
        }
        public virtual ISlotData Equip(ISlotData item,TEnum targetSlot)
        {
            return Equip(item,targetSlot.ToString());
        }
        public virtual ISlotData Equip(ISlotData item,string targetSlot)
        {
            if (item.IsEmpty) throw new ArgumentNullException("Item to equip cannot be null");
            if (item.Item is not TItem) throw new ArgumentException("You can't equip that");
            if (item.StackSize > 1|| item.Item.f_MaxStackSize > 1) throw new ArgumentException("Equipment cannot have a stacks size higher than 1");

            TItem toEquip = (TItem)item.Item;
            targetSlot = targetSlot.ToUpper();
            if (!Enum.Equals(toEquip.f_ValidEquipmentSlot,_config.SlotBindings[targetSlot])) throw new ArgumentException("That equipment can't be equipped in that slot");

            var lastEquip = _equipmentSlots[targetSlot].Insert(new TemporarySlot(toEquip,item.StackSize));

            if (EquipmentConfig.SplitLoadoutKey(targetSlot,out (string name, string slot, int index) output))
            {
                if (_config.Loadouts[output.name].selectedProfile == output.index)
                {
                    //the slot belongs to a loadout and we only equip if this slot is part of the selected loadout
                    if (lastEquip.Item == null) { toEquip.Equip(_context); return TemporarySlot.Empty; }
                    ((TItem)lastEquip.Item).UnEquip(_context);
                    toEquip.Equip(_context);
                    return lastEquip;
                } else {
                    if (lastEquip.Item == null) return TemporarySlot.Empty;
                    return lastEquip;
                }
            }

            if (lastEquip.Item == null) { toEquip.Equip(_context); return TemporarySlot.Empty; }
            ((TItem)lastEquip.Item).UnEquip(_context);
            toEquip.Equip(_context);
            return lastEquip;
        }
        public void ChangeLoadout(string name,int index)
        {
            if (_config.Loadouts.ContainsKey(name))
            {
                var selectedLoadout = _config.Loadouts[name];
                if (selectedLoadout.selectedProfile == index) return;
                if (index >= 0 && index < selectedLoadout.totalLoadouts)
                {
                        selectedLoadout.loadoutBindings.ForEach(slotBinding =>
                    {
                        if (_equipmentSlots.TryGetValue($"{name}-{slotBinding.Key}-{selectedLoadout.selectedProfile}", out var slot))
                        {
                            if (!slot.IsEmpty) slot.Item.UnEquip(_context);
                        }
                    });
                        selectedLoadout.selectedProfile = index;
                        selectedLoadout.loadoutBindings.ForEach(slotBinding =>
                    {
                        if (_equipmentSlots.TryGetValue($"{name}-{slotBinding.Key}-{selectedLoadout.selectedProfile}", out var slot))
                        {
                            if (!slot.IsEmpty) slot.Item.Equip(_context);
                        }
                    });
                } else
                {
                    throw new IndexOutOfRangeException("Provided loadout index is not within the correct range");
                }
            } else
            {
                throw new KeyNotFoundException("The provided loadout name is not a valid loadout key");
            }
        }
        public virtual ISlotData UnEquip(TEnum targetSlot)
        {
            return UnEquip(targetSlot.ToString());
        }
        public virtual ISlotData UnEquip(string targetSlot)
        {
            targetSlot = targetSlot.ToUpper();
            var lastEquip = _equipmentSlots[targetSlot].Eject();
            if (lastEquip.Item == null) return TemporarySlot.Empty;

            if (EquipmentConfig.SplitLoadoutKey(targetSlot,out (string name, string slot, int index) output))
            {
                //the slot belongs to a loadout and we only unequip if this slot is part of the selected loadout
                if (_config.Loadouts[output.name].selectedProfile == output.index)
                {
                    ((TItem)lastEquip.Item).UnEquip(_context);
                }
            } else {
                ((TItem)lastEquip.Item).UnEquip(_context);
            }
            return lastEquip;
        }
        public ISlotData GetSlot(string slotName)
        {
            return this[slotName];
        }
    }
}