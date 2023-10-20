using InventorySystem;
using RPG.Core.Character;
using System.Collections.Generic;

namespace RPG.Core.Events
{
    public class PartyEventArgs : RPGEventArgs
    {
        public ICharacterSelector focusedMember;
        public List<IPartyCharacter> partyMemberData;
        public FilterableInventory partyInventory;

        public PartyEventArgs(ICharacterSelector focused, List<IPartyCharacter> memberData,FilterableInventory inv)
        {
            focusedMember = focused;
            partyMemberData = memberData;
            partyInventory = inv;
        }
    }
}
