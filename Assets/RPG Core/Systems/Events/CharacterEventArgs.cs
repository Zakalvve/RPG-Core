using RPG.Core.Character;
using UnityEngine;

namespace RPG.Core.Events
{
    public class CharacterEventArgs : RPGEventArgs
    {
        public ICharacterData character;
        public CharacterEventArgs(ICharacterData c) : base (c.ID)
        {
            character = c;
        }
    }

    public class CharacterSelectEventArgs : RPGEventArgs
    {
        public GameObject selectedGO;
        public ICharacterSelector characterSelector;

        public CharacterSelectEventArgs(ICharacterSelector s, GameObject go) : base(s.ID)
        {
            selectedGO = go;
            characterSelector = s;
        }
    }

    public class CharacterMoveEventArgs : CharacterEventArgs
    {
        public Vector3 moveTo;
        public CharacterMoveEventArgs(ICharacterData c, Vector3 v) : base(c)
        {
            moveTo = v;
        }
    }
}
