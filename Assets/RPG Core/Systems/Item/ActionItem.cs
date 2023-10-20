using InventorySystem;
using System;
using UnityEngine;

namespace Item
{
    public class ActionItem : BaseItem<E_Action>, IActionItem
    {
        public ActionItem(E_Action data) : base(data) { }

        public void Use(IActionContext context) { context.Use(this); }
    }
}