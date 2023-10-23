using RPG.Core.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Core.UI.Components.UnitFrames
{
    public class UI_PartyFrame : VisualElement
    {
        public class UXMLFactory : UxmlFactory<UI_PartyFrame,UxmlTraits> { }

        public List<UI_UnitFrame> activeMembers = new List<UI_UnitFrame>();
        public Stack<UI_UnitFrame> inactiveSlots = new Stack<UI_UnitFrame>();

        public VisualElement root;

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement visualElement,IUxmlAttributes bag,CreationContext context)
            {
                base.Init(visualElement,bag,context);

                var element = visualElement as UI_PartyFrame;
                element.Clear();

                VisualTreeAsset component = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RPG Core/UI/Components/Unit Frames/PartyFrame.uxml");
                VisualElement partyFrame = component.Instantiate();

                element.root = partyFrame.Q<VisualElement>("party-root");

                partyFrame.Query<UI_UnitFrame>().ForEach(frame =>
                {
                    frame.Hide();
                    element.inactiveSlots.Push(frame);
                    element.root.Remove(frame);
                });

                element.AddToClassList("party-frame");
                element.Add(partyFrame);
            }
        }

        public void AddPartyMember(ICharacterData character)
        {
            if (inactiveSlots.Count == 0) throw new InvalidOperationException("Requested addition of new party memeber, but party is already full");

            var frame = inactiveSlots.Pop();
            frame.AllocateUnit(character);
            activeMembers.Add(frame);
            root.Add(frame);
        }

        public void RemovePartyMember(ICharacterData character)
        {
            var toRemove = activeMembers.First(frame => frame.AllocatedCharacterID == character.ID);
            if (toRemove != null)
            {
                activeMembers.Remove(toRemove);
                toRemove.DeallocateUnit(character);
                root.Remove(toRemove);
            }
        }
    }
}