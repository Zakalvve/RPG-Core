using InventorySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using RPG.Core.Events;
using System;
using RPG.Core.Player;

namespace RPG.Core.Character
{
    public class PartyController : M_Party, IParty
    {
        public List<ICharacterData> PartyMemberData
        {
            get
            {
                return _partyMembers.Select(kvp => kvp.Value.Character).Cast<ICharacterData>().ToList();
            }
        }
        private PartyCharacterController _focusedPartyMember;

        #region Dependancies
        private Dictionary<string,PartyCharacterController> _partyMembers { get; set; } = new Dictionary<string,PartyCharacterController>();
        private IPartyStash _partyInventory;
        private EventEmitter _emitter;
        private ISystemMessenger _messenger;
        #endregion
        
        #region Initialize
        [Inject]
        private void Initialize(List<PartyCharacterController> startingMembers,IPartyStash inventory,EventEmitter emitter,ISystemMessenger messenger)
        {
            //party is singleton entity
            Entity = E_Party.GetEntity(0);

            _partyInventory = inventory;
            _emitter = emitter;
            _messenger = messenger;

            foreach (var member in startingMembers)
            {
                _partyMembers.Add(member.GetComponent<PartyCharacter>().Entity.Id.ToString(),member);
                member.transform.parent = transform;
            }

            _focusedPartyMember = _partyMembers.First().Value;
        }
        private new void Start()
        {
            EmitPartyStart();
            EmitFocus();
        }

        private void OnEnable()
        {
            _emitter.Subscribe<PlayerEvents,CharacterSelectEventArgs>(PlayerEvents.OnPlayerSelectCharacter,OnPlayerSelectCharacter);
            _emitter.Subscribe<PlayerEvents,CharacterMoveEventArgs>(PlayerEvents.OnPlayerMovePartyCharacter,OnPlayerMoveCharacter);
        }
        private void OnDisable()
        {
            _emitter.UnsubscribeFromAll();
        }
                private void EmitAddtAndFocus(PartyCharacterController ctrl)
        {
            EmitAddMember(ctrl);
            EmitFocus();
        }
        #endregion

        #region Events
        void OnPlayerSelectCharacter(object sender,CharacterSelectEventArgs args)
        {
            if (!TrySelectPartyMember(args.characterSelector.ID))
            {
                if (args.selectedGO.TryGetComponent(out PartyCharacterController pcController))
                {
                    TryAddPartyMember(pcController);
                }
            }
        }
        void OnPlayerMoveCharacter(object sender, CharacterMoveEventArgs args)
        {
            TryMovePartyMember(args.moveTo);
        }
        private void EmitAddMember(PartyCharacterController ctrl)
        {
            _emitter.Emit(PartyEvents.OnPartyMemberAdded,this,new CharacterEventArgs(ctrl.Character));
        }
        private void EmitFocus()
        {
            _emitter.Emit(PartyEvents.OnPartyMemberFocused,this,new CharacterEventArgs(_focusedPartyMember.Character));
        }
        private void EmitPartyStart()
        {
            _emitter.Emit(PartyEvents.OnPartyStart,this,new PartyEventArgs(_focusedPartyMember,PartyMemberData,_partyInventory));
        }
        #endregion

        #region IParty
        public ICharacterSelector FirstPartyMember()
        {
            if (_partyMembers.Count <= 0)
            {
                throw new Exception("Party should never be created empty");
            }

            return _partyMembers.First().Value;
        }
        public bool TryMovePartyMember(Vector3 destination)
        {
            if (_focusedPartyMember == null) 
                return false;
            
            MovePartyMember(_focusedPartyMember,destination);

            return true;
        }
        public bool TrySelectPartyMember(string id)
        {
            if (_partyMembers.TryGetValue(id,out PartyCharacterController ctrl))
            {
                _focusedPartyMember = ctrl;
                EmitFocus();
                return true;
            }
            return false;
        }
        public bool TryAddPartyMember(PartyCharacterController crtl)
        {
            if (_partyMembers.ContainsKey(crtl.ID)) return false;

            //A CURRENTLY ARBRITRARY CONDITION FOR JOINING SO REMOVE SOON
            if (Vector3.Distance(crtl.gameObject.transform.position,_focusedPartyMember.gameObject.transform.position) < 3)
            {
                AddPartyMember(crtl);
                return true;
            }
            return false;
        }
        #endregion

        private void MovePartyMember(PartyCharacterController ctrl,Vector3 destination)
        {
            ctrl.MoveTo(destination);
        }
        private void AddPartyMember(PartyCharacterController ctrl)
        {
            ctrl.transform.parent = transform;

            _partyMembers.Add(ctrl.ID, ctrl);
            _focusedPartyMember = ctrl;

            DB_AddMember(ctrl.Character.Name,(E_Character)ctrl.GetComponent<PartyCharacter>().Entity);

            Debug.Log("Party member added");
            _messenger.ShowMessage($"{ctrl.Character.Name} has joined the party.",Color.blue);

            EmitAddtAndFocus(ctrl);
        }
        private void DB_AddMember(string name,E_Character character)
        {
            var party = E_Party.GetEntity(0);
            var newPartyMember = E_Member.NewEntity();

            newPartyMember.f_name = name;
            newPartyMember.f_member = character;
            newPartyMember.f_party = party;

            party.f_Member.Add(newPartyMember);
        }
    }

    public enum PartyEvents
    {
        OnPartyStart,
        OnPartyMemberAdded,
        OnPartyMemberFocused
    }
}