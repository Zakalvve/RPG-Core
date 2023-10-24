using RPG.Core.Events;
using RPG.Core.UI;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using System;
using RPG.Core.Character;
using RPG.Core.Player;

namespace InventorySystem
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(UI_InventoryManager))]
    public class UI_CharacterSheet : MonoBehaviour
    {

        public UI_Panel sheetRootElement;
        private EventEmitter _emitter;
        //Name
        private static TextElement CharName;
        public string charNameRootName = "char-name";

        private VisualElement characterPortrait;
        private string charPortraitRootName = "char-portrait";

        //Inventory
        public IUI_ChangeableInventoryDisplay<IInventory> CharBag;
        public IUI_ChangeableInventoryDisplay<IPartyStash> PartyStash;
        public IUI_ChangeableInventoryDisplay<IEquipmentInventory> CharEquipment;
        


        [Inject]
        public void Initialize(
            UIDocument UI, 
            IUI_ChangeableInventoryDisplay<IInventory> bagDisplay,
            IUI_ChangeableInventoryDisplay<IPartyStash> stashDisplay,
            IUI_ChangeableInventoryDisplay<IEquipmentInventory> equipmentDisplay,
            EventEmitter emitter
            )
        {
            var panelRoot = UI.rootVisualElement;
            sheetRootElement = panelRoot.Q<UI_Panel>();
            sheetRootElement.InitializePanel();

            CharName = sheetRootElement.Q<TextElement>(charNameRootName);
            CharName.text = "";

            characterPortrait = sheetRootElement.Q<VisualElement>(charPortraitRootName);

            CharBag = bagDisplay;
            PartyStash = stashDisplay;
            CharEquipment = equipmentDisplay;
            sheetRootElement.Hide();

            _emitter = emitter;
        }

        public void OnEnable()
        {
            _emitter.Subscribe(PlayerEvents.OnPlayerToggleCharacterSheet,ToggleVisible);
            _emitter.Subscribe<PartyEvents, CharacterEventArgs>(PartyEvents.OnPartyMemberFocused,ChangeCharacter);
            _emitter.Subscribe<PartyEvents, PartyEventArgs>(PartyEvents.OnPartyStart,ChangeParty);
        }
        public void OnDisable()
        {
            _emitter.UnsubscribeFromAll();
        }

        public void ToggleVisible(object sender)
        {
            if (sheetRootElement.IsHidden)
            {
                sheetRootElement.Show();
            }
            else
            {
                sheetRootElement.Hide();
            }
        }
        public void ChangeCharacter(object sender, CharacterEventArgs args)
        {
            CharName.text = args.character.Name;
            characterPortrait.style.backgroundImage = new StyleBackground(args.character.Portrait);
            ChangeBag(args.character.Bags);
            ChangeEquipment(args.character.Equipment);
        }
        public void ChangeParty(object sender, PartyEventArgs args)
        {
            PartyStash.ChangeInventory(args.partyInventory);
        }
        public void ChangeBag(IInventory inv)
        {
            CharBag.ChangeInventory(inv);
        }
        public void ChangeEquipment(IEquipmentInventory inv)
        {
            CharEquipment.ChangeInventory(inv);
        }
    }
}
