using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using InventorySystem;

namespace RPG.Core.ExtensionMethods
{
    public static class Extensions
    {
        public static void OnAllChildren(this VisualElement element,Action<VisualElement> operation)
        {
            operation(element);
            foreach (var child in element.hierarchy.Children())
            {
                OnAllChildren(child,operation);
            }
        }
    }
}
