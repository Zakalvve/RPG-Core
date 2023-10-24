using InventorySystem;
using RPG.Core.Character;
using System.Collections.Generic;

namespace RPG.Core.Events
{
    public class PartyEventArgs : RPGEventArgs
    {
        public ICharacterSelector focusedMember;
        public List<ICharacterData> partyMemberData;
        public IPartyStash partyInventory;

        public PartyEventArgs(ICharacterSelector focused, List<ICharacterData> memberData,IPartyStash inv)
        {
            focusedMember = focused;
            partyMemberData = memberData;
            partyInventory = inv;
        }
    }
}
