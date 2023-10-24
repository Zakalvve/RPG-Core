using RPG.Core.Events;
using RPG.Core.Character;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace InventorySystem
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(UI_InventoryManager))]
    public class UI_ActionBar : MonoBehaviour
    {
        public string actionBarRootName = "action-bar-root";
        public IUI_ChangeableInventoryDisplay<IActionBar> _actionBar;
        private EventEmitter _emitter;

        [Inject]
        public void Initialize(IUI_ChangeableInventoryDisplay<IActionBar> actionBar, EventEmitter emitter)
        {
            _actionBar = actionBar;
            _emitter = emitter;
        }
        public void OnEnable()
        {
            _emitter.Subscribe<PartyEvents, CharacterEventArgs>(PartyEvents.OnPartyMemberFocused,ChangeCharacter);
        }
        private void OnDisable()
        {
            _emitter.UnsubscribeFromAll(); 
        }

        public void ChangeCharacter(object sender,CharacterEventArgs args)
        {
            ChangeActionBar(args.character.ActionBar);
        }
        public void ChangeActionBar(IActionBar inv)
        {
            _actionBar.ChangeInventory(inv);
        }
    }
}
