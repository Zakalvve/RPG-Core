using RPG.Core.Character;
using UnityEngine;

namespace RPG.Core.Events
{
    public class CharacterEventArgs : RPGEventArgs
    {
        public IPartyCharacter character;
        public CharacterEventArgs(IPartyCharacter c) : base (c.ID)
        {
            character = c;
        }
    }

    public class CharacterSelectEventArgs : CharacterEventArgs
    {
        public GameObject selectedGO;
        public ICharacterSelector characterSelector;

        public CharacterSelectEventArgs(IPartyCharacter c,ICharacterSelector s, GameObject go) : base(c)
        {
            selectedGO = go;
            characterSelector = s;
        }
    }

    public class CharacterMoveEventArgs : CharacterEventArgs
    {
        public Vector3 moveTo;
        public CharacterMoveEventArgs(IPartyCharacter c, Vector3 v) : base(c)
        {
            moveTo = v;
        }
    }
}
