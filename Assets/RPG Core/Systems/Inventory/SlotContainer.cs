using BansheeGz.BGDatabase;
using Item;
using RPG.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace InventorySystem
{
    public class SlotContainer<TItem> : ISlotContainerList<TItem> where TItem : class, IItem
    {
        public SlotContainer(List<E_InventorySlot> dbSlots)
        {
            _capacity = dbSlots.Count;
            Initialize(dbSlots);
        }

        private int _firstFree;
        private int _capacity;
        private int _count;
        private List<Slot> _slots;
        private List<int> _items;
        public int Capacity => _capacity;
        public int Count => _count;
        public bool IsEmpty => _count == 0;
        public bool IsFull => _count >= _capacity;
        protected IEnumerable<Slot> Slots => _slots;
        protected int FirstFree => _firstFree;
        public virtual ISlotData this[int index]
        {
            get
            {
                if (index < 0 || index >= _slots.Count) throw new ArgumentOutOfRangeException("Index out of bounds of array");
                return _slots[index].Peek();
            }
        }

        [MemberNotNull(nameof(_items))]
        [MemberNotNull(nameof(_slots))]
        private void Initialize(List<E_InventorySlot> dbSlots)
        {
            _slots = new List<Slot>(dbSlots.Count);
            _items = new List<int>(dbSlots.Count);
            for (int i = 0; i < dbSlots.Count; i++)
            {
                _slots.Add(new Slot(i,dbSlots[i]));
            }
        }
        public virtual ISlotData Insert(ISlotData slot)
        {
            return Insert(slot,_firstFree);
        }
        public virtual ISlotData Insert(ISlotData slot,int at)
        {
            if (at < 0 || at >= _slots.Count) throw new ArgumentOutOfRangeException("Index out of bounds of array");

            Slot internalSlot = _slots[at];

            bool slotWasEmpty = internalSlot.IsEmpty;
            ISlotData output = internalSlot.Insert(slot);
            bool slotNowFilled = !internalSlot.IsEmpty;

            if (slotWasEmpty && slotNowFilled)
            {
                _count++;
                _items.Add(at);

                //we used the first free slot and must calculate the next one
                if (at == _firstFree)
                {
                    if (!IsFull)
                    {
                        for (int i = _firstFree; i < _slots.Count; i++)
                        {
                            if (_slots[i].IsEmpty)
                            {
                                _firstFree = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //by setting first free to the last index in the container when it is full, the index of the next ejected slot will become the new first free
                        _firstFree = _capacity - 1;
                    }
                }
            }

            return output;
        }
        protected void Swap(int x, int y)
        {
            if (_slots[x].IsEmpty && _slots[y].IsEmpty) return;
            if (_slots[x].IsEmpty)
            {
                Insert(_slots[y].Eject(),x);
                return;
            } else if (_slots[y].IsEmpty)
            {
                Insert(_slots[x].Eject(),y);
                return;
            }
            var swap = Eject(y);
            Insert(Eject(x),y);
            Insert(swap,x);
        }
        public virtual ISlotData Eject(int from,int stacksToRemove = int.MaxValue)
        {
            if (from < 0 || from >= _slots.Count) throw new ArgumentOutOfRangeException("Index out of bounds of array");
            Slot internalSlot = _slots[from];

            bool slotWasFilled = !internalSlot.IsEmpty;
            ISlotData output = stacksToRemove == int.MaxValue ? internalSlot.Eject() : internalSlot.Eject(stacksToRemove);
            bool slotNowEmpty = internalSlot.IsEmpty;

            if (slotWasFilled && slotNowEmpty)
            {
                //We fully ejected the slot and must maintain the state of the collection
                bool found = false;
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i] == from)
                    {
                        found = true;
                        _items.RemoveAt(i);
                        continue;
                    }
                }
                Debug.Assert(found);

                _count--;
                if (from < _firstFree) _firstFree = from;
            }
            return output;
        }
        public int IndexOfFirst(TItem item)
        {
            int index = -1;
            for (int i = 0; i < _items.Count; i++)
            {
                if (_slots[_items[i]].Peek().Item.ID == item.ID)
                {
                    if (index == -1) 
                    { 
                        index = i; 
                        continue; 
                    }
                    if (_items[i] < index) 
                        index = i;
                }
            }

            return index == -1 ? index : _items[index];
        }
        public IEnumerable<int> IndexOfAll(TItem item)
        {
            List<int> found = new List<int>();

            for (int i = 0; i < _items.Count; i++)
            {
                if (_slots[_items[i]].Peek().Item.ID == item.ID)
                {
                    found.Add(_items[i]);
                }
            }

            found.Sort();
            return found;
        }

        public sealed class Slot : ISlotData
        {
            [MemberNotNullWhen(false,nameof(Item))]
            public bool IsEmpty => Item == null && StackSize == -1 ? true : false;
            public int StackSize { get { return _data.f_StackSize; } private set { _data.f_StackSize = value; } }
            IItem ISlotData.Item => Item;
            private E_InventorySlot _data;
            private int _index;
            public int Index => _index;

            internal Slot(int index,E_InventorySlot data)
            {
                if (data == null) throw new ArgumentException("Source data is null.");
                _data = data;
                _index = index;

                if (data.f_Item == null)
                {
                    Item = null;
                }
                else
                {
                    if (_data.f_StackSize < 1)
                    {
                        UnityEngine.Debug.LogWarning($"Loaded an item from an inventory slot whose stack size was ${_data.f_StackSize}. Setting it to 1 instead.");
                        _data.f_StackSize = 1;
                    }
                    Item = Factories.CreateItem<TItem>(data.f_Item);
                }

                StackSize = data.f_StackSize;

                //create listeners for when changes are made in the database
                _data.Meta.AddEntityUpdatedListener(_data.Id,OnDbChanged);
                EventEmitter.OnReset += RemoveDbListeners;
            }

            ~Slot()
            {
                RemoveDbListeners();
            }

            void OnDbChanged(object sender,BGEventArgsEntityUpdated args)
            {
                if (args.FieldId == _data.Meta.GetFieldId("Item"))
                {
                    if (_data.f_Item == null) return;
                    if (Item != null && Item.ID == _data.f_Item.Id.ToString()) return;
                    Set(Factories.CreateItem<TItem>(_data.f_Item));
                }
                else if (args.FieldId == _data.Meta.GetFieldId("StackSize"))
                {
                    Set(_data.f_StackSize);
                }
            }
            private void RemoveDbListeners()
            {
                _data.Meta.RemoveEntityUpdatedListener(_data.Id,OnDbChanged);
            }

#nullable enable
            public TItem? Item { get { return _item; } private set { _item = value; _data.f_Item = _item?.Item ?? null; } }
            private TItem? _item;
            public void Set(TItem? item,int stacks)
            {
                Item = item;
                StackSize = stacks;
                ObjectChanged?.Invoke(this);
            }
            public void Set(TItem? item)
            {
                Item = item;
                ObjectChanged?.Invoke(this);
            }
            public void Set(int stacks)
            {
                StackSize = stacks;
                ObjectChanged?.Invoke(this);
            }
#nullable disable

            public event Action<ISlotData> ObjectChanged;
            public void Bind(Action<ISlotData> handleObjectChanged)
            {
                ObjectChanged += handleObjectChanged;
                ObjectChanged?.Invoke(this);
            }
            public void Unbind(Action<ISlotData> handleObjectChanged)
            {
                ObjectChanged -= handleObjectChanged;
            }
            internal ISlotData Peek() => this;
            public ISlotData Insert(int stacksToAdd) => Insert(new TemporarySlot(Item,stacksToAdd));
            public ISlotData Insert(ISlotData slotData)
            {
                if (slotData.StackSize < 1) throw new ArgumentOutOfRangeException("Cannot insert slot with stack size less than 1");
                if (slotData.Item == null) throw new ArgumentNullException("Cannot insert a null item");
                if (slotData.StackSize > slotData.Item.f_MaxStackSize && slotData.Item != Item && Item != null) throw new ArgumentException("When performing an slot swap the provided stack size must be less than or equal to the slots items max stack size.");

                int amountOver;

                if (IsEmpty)
                {
                    if (slotData.StackSize <= slotData.Item.f_MaxStackSize)
                    {
                        Set((TItem)slotData.Item,slotData.StackSize);
                        Debug.Assert(!IsEmpty);
                        return TemporarySlot.Empty;
                    }
                    Set((TItem)slotData.Item, 0);
                }

                if (slotData.Item.ID != Item.ID)
                {
                    //We are swapping the slot data with the new data
                    ISlotData output = IsEmpty ? TemporarySlot.Empty : new TemporarySlot(Item,StackSize);
                    Set((TItem)slotData.Item,slotData.StackSize);
                    Debug.Assert(!IsEmpty);
                    return output;
                }

                amountOver = ChangeStackSize(slotData.StackSize);
                if (amountOver < 1) return TemporarySlot.Empty;
                return new TemporarySlot(Item,amountOver);
            }
            public ISlotData Eject()
            {
                if (IsEmpty) return TemporarySlot.Empty;
                ISlotData data = new TemporarySlot(Item,StackSize);
                Clear();
                return data;
            }
            public ISlotData Eject(int stacksToRemove)
            {
                if (IsEmpty) return TemporarySlot.Empty;
                if (stacksToRemove >= StackSize)
                {
                    return Eject();
                }

                ChangeStackSize(Math.Abs(stacksToRemove) * (-1));
                return new TemporarySlot(Item,stacksToRemove);
            }
            private void Clear()
            {
                Set(null,-1);
                Debug.Assert(Item == null);
                Debug.Assert(StackSize == -1);
            }
            private int ChangeStackSize(int amount)
            {
                if (IsEmpty) throw new InvalidOperationException("Cannot change the stack size when the slot is empty");
                if (StackSize + amount <= 0) throw new ArgumentOutOfRangeException("Resulting stack size cannot be below 1");
                int amountOver = 0, initialStacks = StackSize + amount;
                if (amount + StackSize > Item.f_MaxStackSize)
                {
                    amountOver = amount - Item.f_MaxStackSize + StackSize;
                    amount -= amountOver;
                }
                Set(StackSize + amount);
                Debug.Assert(initialStacks == StackSize + amountOver);
                return amountOver;
            }
        }
    }

    public class TemporarySlot : ISlotData
    {
        public TemporarySlot(IItem item,int stackSize)
        {
            (Item, StackSize) = (item, stackSize);
        }

        public static TemporarySlot Empty => new TemporarySlot(null,-1);
        public IItem Item { get; set; }

        public int StackSize { get; set; }

        public bool IsEmpty => Item == null && StackSize == -1 ? true : false;

        public void Bind(Action<ISlotData> handleObjectChanged)
        {
            throw new InvalidOperationException("Temporary slots cannot be bound");
        }

        public void Unbind(Action<ISlotData> handleObjectChanged)
        {
            throw new InvalidOperationException("Temporary slots cannot be bound");
        }
    }
}