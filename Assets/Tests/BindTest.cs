using System.Collections;
using InventorySystem;
using Item;
using RPG.Core;
using UnityEngine;

public class BindTest : MonoBehaviour
{
    //public BindableProperty<int> myint;
    //public BindableProperty<Sprite> mySprite;
    //public BindableProperty<ISlotData> myItemSlot;
    //public Inventory inv;

    //public TestSpritesList sprites;


    //public void Awake()
    //{
    //    inv = new Inventory(1);
    //    inv.Insert(InventoryFactory.CreateItemSlot(new BaseItem("Random item", 10,sprites.Sprites[0]), 5));
    //    myItemSlot.Value = inv[0];

    //    StartCoroutine(ChangeIntValue());
    //    StartCoroutine(ChangeSpriteValue());
    //    StartCoroutine(ChangeSlotData());
    //}

    //public IEnumerator ChangeIntValue()
    //{
    //    while (true)
    //    {
    //        myint.Value = new System.Random().Next(0,sprites.Sprites.Count);
    //        yield return new WaitForSeconds(1);
    //    }
    //}

    //public IEnumerator ChangeSpriteValue()
    //{
    //    while (true)
    //    {
    //        mySprite.Value = sprites.Sprites[myint.Value];
    //        yield return new WaitForSeconds(1.5f);
    //    }
    //}

    //public IEnumerator ChangeSlotData()
    //{
    //    while (true)
    //    {
    //        inv.Insert(InventoryFactory.CreateItemSlot(new BaseItem("Random item",10,sprites.Sprites[0]),1), 0);
    //        myItemSlot.Value = inv[0];
    //        yield return new WaitForSeconds(2.3f);
    //    }
    //}
}
