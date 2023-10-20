using RPG.Core.Character;
using RPG.Core.Events;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace RPG.Core.UI.Components.UnitFrames
{
    public class PartyFrameController : MonoBehaviour
    {

        EventEmitter _emitter;
        UI_PartyFrame frame;

        [Inject]
        void Initialize(UIDocument doc,EventEmitter emitter)
        {
            frame = doc.rootVisualElement.Q<UI_PartyFrame>("party-frame");
            _emitter = emitter;
            _emitter.Subscribe<PartyEvents, CharacterEventArgs>(PartyEvents.OnPartyMemberAdded,OnPartyMemberAdded);
            _emitter.Subscribe<PartyEvents, PartyEventArgs>(PartyEvents.OnPartyStart,OnPartyFormed);
        }

        void OnPartyMemberAdded(object sender, CharacterEventArgs args)
        {
            AddPartyMember(args.character);
        }

        void OnPartyMemberRemoved(object sender, CharacterEventArgs args)
        {
            RemovePartyMember(args.character);
        }

        void OnPartyFormed(object sender, PartyEventArgs args)
        {
            foreach (var character in args.partyMemberData)
            {
                AddPartyMember(character);
            }
        }

        void AddPartyMember(IPartyCharacter character)
        {
            frame.AddPartyMember(character);
        }

        void RemovePartyMember(IPartyCharacter character)
        {
            frame.RemovePartyMember(character);
        }
    }
}