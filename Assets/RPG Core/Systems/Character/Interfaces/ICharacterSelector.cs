using Assets.RPG_Core.Core;

namespace RPG.Core.Character
{
    public interface ICharacterSelector : IIDable
    {
        ICharacterData Character { get; }
        void Select();
        void Deselect();
    }
}
