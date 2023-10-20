using UnityEngine;

namespace Item
{
    public class BaseItem<T> : IItem where T : E_v_item
    {
        protected T _data;
        public BaseItem(T data)
        {
            _data = data;
        }
        public E_v_item Item => _data;
        public string ID { get { return _data.Id.ToString(); } }
        public string f_name { get { return _data.f_name; } }
        public string f_Type { get { return _data.f_Type; } }
        public int f_MaxStackSize { get { return _data.f_MaxStackSize; } }
        public Sprite f_Icon { get { return _data.f_Icon; } }
        public string f_Description { get { return _data.f_Description; } }
    }
}