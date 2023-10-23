using UnityEngine;
using Zenject;

namespace RPG.Core.Character {
    public class PartyCharacterController : MonoBehaviour, ICharacterSelector
    {
        #region Dependancies
        private PartyCharacter _character;
        private ISystemMessenger _messenger;
        private MeshRenderer _renderer;
        private PawnMovementController _moveController;
        #endregion

        #region Initialize
        [Inject]
        public void Initialize(PartyCharacter character, ISystemMessenger messenger,MeshRenderer renderer,PawnMovementController moveController)
        {
            _character = character;
            _messenger = messenger;
            _renderer = renderer;
            _moveController = moveController;
        }
        #endregion

        #region ICharacterSelector
        public string ID => _character.ID;
        public ICharacterData Character => _character;
        public void Select()
        {
            _messenger.ShowMessage($"{_character.Name} selected",Color.green);
            _renderer.material.color = Color.green;
        }
        public void Deselect()
        {
            _renderer.material.color = Color.white;
        }
        #endregion

        public void MoveTo(Vector3 destination)
        {
            _moveController.MoveTo(destination);
        }
    }
}