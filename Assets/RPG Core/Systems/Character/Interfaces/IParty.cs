using UnityEngine;

namespace RPG.Core.Character
{
    public interface IParty
    {
        ICharacterSelector FirstPartyMember();
        bool TryMovePartyMember(Vector3 destination);
        bool TryAddPartyMember(PartyCharacterController partyMember);
        bool TrySelectPartyMember(string ID);
    }
}
