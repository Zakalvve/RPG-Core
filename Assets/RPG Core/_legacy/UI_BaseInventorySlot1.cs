//using ExtensionMethods;
//using System.Drawing;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace InventorySystem
//{
//    public class UI_BaseInventorySlot : VisualElement
//    {
//        public new class UxmlFactory : UxmlFactory<UI_BaseInventorySlot> { }

//        protected Image _icon;
//        protected TextElement _stacks;
//        protected static Sprite defaultSprite = null;
//        public UI_BaseInventorySlot() : base()
//        {
//            name = "ghostIcon";

//            AddToClassList("inventory-slot");
//            AddToClassList("ghost-icon"); //only a ghost icon can be created without parameters

//            style.height = 0;
//            style.width = 75;
//            style.paddingTop = 75;

//            _icon = new Image();
//            _icon.AddToClassList("stretch-to-parent-size");
//            _icon.AddToClassList("slot-icon");
//            _icon.AddToClassList("inventory-slot-stacks-container");

//            _stacks = new TextElement();
//            _stacks.AddToClassList("inventory-slot-stacks");

//            style.top = 0;
//            style.left = 0;

//            _icon.Add(_stacks);
//            Add(_icon);
//        }
//        public UI_BaseInventorySlot(string containerClassName) : base()
//        {
//            AddToClassList("inventory-slot");
//            AddToClassList(containerClassName);

//            _icon = new Image();
//            _icon.AddToClassList("stretch-to-parent-size");
//            _icon.AddToClassList("slot-icon");
//            _icon.AddToClassList("inventory-slot-stacks-container");

//            _stacks = new TextElement();
//            _stacks.AddToClassList("inventory-slot-stacks");

//            _icon.Add(_stacks);
//            Add(_icon);
//        }

//        public void Update(ISlotData d)
//        {
//            if (d.IsEmpty)
//            {
//                _icon.sprite = defaultSprite;
//                _stacks.style.visibility = Visibility.Hidden;
//            }
//            else
//            {
//                _icon.sprite = d.Item.Icon;
//                _stacks.style.visibility = d.StackSize > 1 ? Visibility.Visible : Visibility.Hidden;
//                _stacks.text = d.StackSize.ToString();
//            }
//        }
//        public void Set(string stacks, Sprite s)
//        {
//            _stacks.style.visibility = Visibility.Visible;
//            _stacks.text = stacks;
//            _icon.sprite = s;
//        }
//        public void SetVisibile(Visibility v)
//        {
//            this.OnAllChildren((e) =>
//            {
//                e.style.visibility = v;
//            });
//        }
//    }
//}