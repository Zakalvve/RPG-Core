using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using RPG.Core.Character;
using System;
using RPG.Core.Character.Attributes;

namespace RPG.Core.UI.Components.UnitFrames
{
    public class UI_UnitFrame : VisualElement, IHideable
    {
        public class UXMLFactory : UxmlFactory<UI_UnitFrame,UxmlTraits> { }

        private VisualElement root;
        private UI_ResourceBar healthBar;

        public int width { get; set; }

        public bool IsHidden { get; private set; }

        public string AllocatedCharacterID { get; private set; }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription m_width = new UxmlIntAttributeDescription() { name = "width",defaultValue = 100 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement visualElement,IUxmlAttributes bag,CreationContext context)
            {
                base.Init(visualElement,bag,context);

                var element = visualElement as UI_UnitFrame;
                element.width = m_width.GetValueFromBag(bag,context);
                element.Clear();

                VisualTreeAsset component = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RPG Core/UI/Components/Unit Frames/PartyUnitFrame.uxml");
                VisualElement unitFrame = component.Instantiate();
                
                element.root = unitFrame.Q<VisualElement>("unit-frame-root");
                element.healthBar = unitFrame.Q<UI_ResourceBar>("health-bar");
                element.style.width = Length.Percent(element.width);

                element.AddToClassList("party-unity-frame");
                element.Add(unitFrame);
            }
        }

        public void AllocateUnit(ICharacterData character)
        {
            //bind character
            AllocatedCharacterID = character.ID;
            var health = character.Stats.Health;
            health.Health.Bind(OnHealthChanged);
            Update(character);
            Show();
        }

        public void DeallocateUnit(ICharacterData character)
        {
            if (character.ID != AllocatedCharacterID) throw new InvalidOperationException("Trying to deallocate character, but character ID does not match allocated character ID");
            //unbind character
            AllocatedCharacterID = String.Empty;
            var health = character.Stats.Health;
            health.Health.Unbind(OnHealthChanged);
            Hide();
        }

        public void Update(ICharacterData character)
        {
            root.style.backgroundImage = new StyleBackground(character.Portrait);
            var health = character.Stats.Health;
            healthBar.value = (int)health.Health.CurrentValue;
            healthBar.maxValue = (int)health.Health.Value;
            //healthBar.value = character.CurrentHP;
            //healthBar.maxValue = character.MaxHP;
        }

        void OnHealthChanged(IRPGVital health)
        {
            healthBar.value = (int)health.CurrentValue;
            healthBar.maxValue = (int)health.Value;
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
            IsHidden = false;
        }

        public void Hide()
        {
            style.display = DisplayStyle.None;
            IsHidden = true;
        }
    }
        
}