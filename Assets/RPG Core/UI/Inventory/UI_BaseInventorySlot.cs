using RPG.Core.ExtensionMethods;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventorySystem
{
    public sealed class UI_BaseInventorySlot : VisualElement, IUI_InventorySlot
    {
        public new class UxmlFactory : UxmlFactory<UI_BaseInventorySlot> { }

        public Image Icon { get; set; }
        public TextElement Stacks { get; set; }

        private static Sprite defaultSprite = null;
        public UI_BaseInventorySlot() : this("inventory-slot-container") { }
        public UI_BaseInventorySlot(string containerClassName) : base()
        {
            AddToClassList("inventory-slot");
            AddToClassList(containerClassName);
            AddToClassList("draggable");

            style.height = 0;
            

            Icon = new Image();
            Icon.AddToClassList("stretch-to-parent-size");
            Icon.AddToClassList("slot-icon");
            Icon.AddToClassList("inventory-slot-stacks-container");

            Stacks = new TextElement();
            Stacks.AddToClassList("inventory-slot-stacks");

            Icon.Add(Stacks);
            Add(Icon);
        }

        public void Update(ISlotData d)
        {
            if (d.IsEmpty)
            {
                Icon.sprite = defaultSprite;
                Stacks.style.visibility = Visibility.Hidden;
            }
            else
            {
                Icon.sprite = d.Item.f_Icon;
                Stacks.style.visibility = d.StackSize > 1 ? Visibility.Visible : Visibility.Hidden;
                Stacks.text = d.StackSize.ToString();
            }
        }

        public void SetVisibile(Visibility v)
        {
            this.OnAllChildren((e) =>
            {
                e.style.visibility = v;
            });
        }
    }
}