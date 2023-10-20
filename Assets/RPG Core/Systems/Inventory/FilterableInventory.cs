using Item;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySystem
{
    public class FilterableInventory : Inventory
    {
        public FilterableInventory(List<E_InventorySlot> dbSlots) : base(dbSlots)
        {
            _filteredInventory = Slots.ToList();
        }

        public event Action<FilterableInventory> OnBeforeFilter;
        public event Action<FilterableInventory> OnAfterFilter;

        private List<Slot> _filteredInventory;
        public List<ISlotData> FilteredInventory => _filteredInventory.Select(slot => (ISlotData)slot.Peek()).ToList();

        public string nameFilter = "";
        public List<string> typeFilter = new List<string>();
        public bool IsFiltered => !String.IsNullOrEmpty(nameFilter) || typeFilter.Count > 0;
        public override ISlotData this[int index]
        {
            get
            {
                if (index < 0 || index >= FilteredInventory.Count) throw new ArgumentOutOfRangeException("Index out of bounds of array");
                return _filteredInventory.ElementAt(index).Peek();
            }
        }
        public override ISlotData Eject(int from,int stacksToRemove = int.MaxValue)
        {
            var output = base.Eject(_filteredInventory[from].Index);
            if (IsFiltered) Filter();
            return output;
        }

        public override ISlotData Insert(ISlotData slot,int at)
        {
            if (IsFiltered)
            {
                if (at >= 0 && at < Capacity)
                {
                    var output = base.Insert(slot,FirstFree);
                    Filter();
                    return output;
                }
            }
            return base.Insert(slot,at);
        }
        public override void SortByType()
        {
            base.SortByType();
            Filter();
        }

        private void Filter()
        {
            OnBeforeFilter?.Invoke(this);
            if (!IsFiltered)
            {
                ResetFilter();
                return;
            }

            _filteredInventory = Slots.Where(slot =>
            {
                return slot.IsEmpty == false
                && slot.Item.f_name.ToUpper().Contains(nameFilter.ToUpper())
                && SlotHasType(slot.Item);

            }).ToList();
            OnAfterFilter?.Invoke(this);
        }
        private bool SlotHasType(IItem item)
        {
            if (typeFilter.Count <= 0) return true;
            foreach (var type in typeFilter)
            {
                if (item.f_Type != type) return false;
            }
            return true;
        }
        private void ResetFilter()
        {
            _filteredInventory = Slots.ToList();
            OnAfterFilter?.Invoke(this);
        }
        public void FilterByName(string name)
        {
            nameFilter = name;
            Filter();
        }
        public void FilterByType(string type)
        {
            if (type == String.Empty) return;
            if (typeFilter.Contains(type))
            {
                RemoveFilterByType(type);
                return;
            }
            typeFilter.Add(type);
            Filter();
        }
        private void RemoveFilterByType(string type)
        {
            if (type == String.Empty) return;
            typeFilter.Remove(type);
            Filter();
        }
        public void RemoveFilters()
        {
            nameFilter = String.Empty;
            typeFilter.Clear();
            Filter();
        }
    }
}
