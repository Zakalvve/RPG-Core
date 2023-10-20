using Assets.RPG_Core.Core;
using BansheeGz.BGDatabase;
using ModestTree;
using RPG.Core.Character;
using RPG.Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace RPG.Core.Player
{
    public class PlayerController : M_Player, IIDable, BGAddonSaveLoad.BeforeSaveReciever
    {
     
        public string ID => EntityId.ToString();
        private ICharacterSelector selectedCharacter;

        #region Dependancies
        private EventEmitter _emitter;
        Camera _playerCamera;
        private PlayerInput _playerInput;
        ISystemMessenger _messenger;
        private SaveLoad save_loader;
        #endregion


        private Vector2 mousePosition;
        private InputAction toggleCharacterSheetAction;
        private InputAction clickAction;
        private InputAction rightClickAction;
        private InputAction mouseMoveAction;
        private InputAction saveAction;
        private InputAction loadAction;

        #region Initialize
        [Inject]
        public void Initialize(EventEmitter emitter, Camera playerCamera, PlayerInput playerInput,  ISystemMessenger messenger)
        {
            _emitter = emitter;
            _playerCamera = playerCamera;
            _playerInput = playerInput;
            _messenger = messenger;

            //remove this dependancy
            save_loader = GetComponent<SaveLoad>();
            Assert.That(save_loader != null);

            toggleCharacterSheetAction = _playerInput.actions["OpenCharacterSheet"];
            clickAction = _playerInput.actions["Click"];
            rightClickAction = _playerInput.actions["RightClick"];
            mouseMoveAction = _playerInput.actions["MouseMove"];
            saveAction = _playerInput.actions["QuickSave"];
            loadAction = _playerInput.actions["QuickLoad"];
        }
        private new void Start()
        {
            transform.position = f_position;
        }
        private void OnEnable()
        {
            toggleCharacterSheetAction.performed += OnToggleCharacterSheet;
            clickAction.performed += OnClick;
            rightClickAction.performed += OnRightClick;
            mouseMoveAction.performed += OnMouseMove;
            saveAction.performed += OnSave;
            loadAction.performed += OnLoad;
            _emitter.Subscribe<PartyEvents,PartyEventArgs>(PartyEvents.OnPartyStart,OnPartyStart);
        }
        private void OnDisable()
        {
            toggleCharacterSheetAction.performed -= OnToggleCharacterSheet;
            clickAction.performed -= OnClick;
            rightClickAction.performed -= OnRightClick;
            saveAction.performed -= OnSave;
            loadAction.performed -= OnLoad;
            _emitter.UnsubscribeFromAll();
        }
        #endregion

        #region EventHandlers
        private void OnSave(InputAction.CallbackContext context)
        {
            _messenger.ShowMessage("Saving Game...", Color.magenta);
            save_loader.Save();
            _messenger.ShowMessage("Game Saved...",Color.magenta);
        }

        private void OnLoad(InputAction.CallbackContext context)
        {
            _messenger.ShowMessage("Loading Game...",Color.magenta);
            save_loader.Load();
        }
        private void OnClick(InputAction.CallbackContext context)
        {
            Ray ray = _playerCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray,out RaycastHit hit))
            {
                if (hit.transform.gameObject.TryGetComponent(out PartyCharacterController character))
                {
                    //Artifical and to be removed
                    if (ChangeCharacter(character))
                    {
                        _emitter.Emit(PlayerEvents.OnPlayerSelectCharacter,this,new CharacterSelectEventArgs(selectedCharacter.Character, character, character.gameObject));
                    }
                }
            }
        }
        private void OnRightClick(InputAction.CallbackContext context)
        {
            Ray ray = _playerCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray,out RaycastHit hit))
            {
                _emitter.Emit(PlayerEvents.OnPlayerMovePartyCharacter,this,new CharacterMoveEventArgs(selectedCharacter.Character,hit.point));
            }
        }

        private void OnPartyStart(object sender,PartyEventArgs args)
        {
            selectedCharacter = args.focusedMember;
            ChangeCharacter(selectedCharacter);
        }

        private void OnToggleCharacterSheet(InputAction.CallbackContext context)
        {
            _emitter.Emit(PlayerEvents.OnPlayerToggleCharacterSheet,this);
        }
        private void OnMouseMove(InputAction.CallbackContext context)
        {
            mousePosition = context.ReadValue<Vector2>();
        }
        #endregion

        private bool ChangeCharacter(ICharacterSelector newCharacterSelector)
        {
            if (selectedCharacter == null) return false;

            selectedCharacter?.Deselect();

            selectedCharacter = newCharacterSelector;

            selectedCharacter.Select();

            return true;
        }

        public void OnBeforeSave()
        {
            if(!gameObject.activeInHierarchy) return;

            if (!gameObject.scene.IsValid()) return;

            f_position = transform.position;
            f_scene = E_Scene.FindEntity(scene => string.Equals(scene.Name,SceneManager.GetActiveScene().name));
        }
    }

    public enum PlayerEvents
    {
        OnPlayerSave,
        OnPlayerLoad,
        OnPlayerSelectCharacter,
        OnPlayerMovePartyCharacter,
        OnPlayerToggleCharacterSheet
    }

    // Events Emit
    // OnPlayerSave                     args: none
    // OnPlayerLoad                     args: none
    // OnPlayerSelectCharacter          args: CharacterSelectEventArgs
    // OnPlayerMoveCharacter            args: CharacterMoveArgs
    // OnPlayerToggleCharacterSheet     args: none

    // Events Subscribe
    // OnPartyStart                     args: PartyEventArgs
}