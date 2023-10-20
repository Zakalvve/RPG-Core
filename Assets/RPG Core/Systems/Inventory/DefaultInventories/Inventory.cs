using System;
using System.Collections.Generic;
using System.Linq;
using Item;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class Inventory : SlotContainer<IItem>
    {
        public Inventory(List<E_InventorySlot> dbSlots) : base(dbSlots) { }
        public virtual ISlotData InsertAll(ISlotData itemSlot)
        {
            if (itemSlot.Item == null) throw new ArgumentNullException("Cannot insert a null item");
            if (itemSlot.StackSize <= 0) throw new ArgumentException("Stack size must be 1 or more when inserting");

            var output = itemSlot;
            if (IndexOfFirst(itemSlot.Item) != -1)
            {
                foreach (int slotIndex in IndexOfAll(itemSlot.Item))
                {
                    output = Insert(output,slotIndex);
                    if (output.IsEmpty) return output;
                }
            }

            while (Count < Capacity)
            {
                output = Insert(output);
                if (output.IsEmpty) return output;
            }

            return output;
        }
        public virtual IEnumerable<ISlotData> InsertAll(IEnumerable<ISlotData> items)
        {
            List<ISlotData> output = new List<ISlotData>();
            foreach (var item in items)
            {
                if (!IsFull)
                {
                    var leftOver = InsertAll(item);
                    if (!leftOver.IsEmpty) output.Add(leftOver);
                } else
                {
                    output.Add(item);
                }
            }
            return output;
        }
        public virtual void SortByType()
        {
            bool wasSwapped = false;
            do
            {
                wasSwapped = false;
                for (int i = 0, j=1; j < Capacity; i++, j++)
                {
                    if (this[j].IsEmpty) continue;
                    if (this[i].IsEmpty && !this[j].IsEmpty)
                    {
                        Swap(i,j);
                        wasSwapped = true;
                        continue;
                     } else if (this[i].Item.f_Type.CompareTo(this[j].Item.f_Type) > 0)
                    {
                        Swap(i,j);
                        wasSwapped = true;
                        continue;
                    }
                    else if (this[i].Item.f_Type.CompareTo(this[j].Item.f_Type) == 0 && this[i].Item.f_name.CompareTo(this[j].Item.f_name) > 0)
                    {
                        Swap(i,j);
                        wasSwapped = true;
                        continue;
                    }
                }
            } while (wasSwapped);
        }
    }
}