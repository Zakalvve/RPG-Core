using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMonobehaviour : MonoBehaviour
{
    public string id;
    public SaveLoad saveload;
    public PlayerInput input;
    InputAction save;
    InputAction load;
    InputAction scroll;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        save = input.actions["QuickSave"];
        load = input.actions["QuickLoad"];
        scroll = input.actions["ZoomCamera"];

        saveload = GetComponent<SaveLoad>();

        List<E_v_item> temp = new List<E_v_item>();

        E_Action.ForEachEntity(action =>
        {
            temp.Add(action);
        });

        E_Item.ForEachEntity(item =>
        {
            temp.Add(item);
        });

        var test = Test<IItem>(temp);
    }

    private void OnEnable()
    {
        save.performed += OnSave;
        load.performed += OnLoad;
        scroll.performed += OnScroll;
    }

    private void OnDisable()
    {
        save.performed -= OnSave;
        load.performed -= OnLoad;
        scroll.performed -= OnScroll;
    }

    void OnSave(InputAction.CallbackContext context)
    {
        saveload.Save();
    }

    void OnLoad(InputAction.CallbackContext context)
    {
        saveload.Load();
    }

    void OnScroll(InputAction.CallbackContext context)
    {
        var scroll = context.ReadValue<float>();
        if (scroll > 0)
        {
            
        } else if (scroll < 0)
        {
            
        }
        Debug.Log("Log a value");
    }

    List<TItem> Test<TItem>(List<E_v_item> data) where TItem : class, IItem
    {
        List<TItem> test =  new List<TItem>();

        foreach (var itemData in data)
        {
            if (itemData is E_Action)
            {
                test.Add((TItem)(IItem)(new ActionItem((E_Action)itemData)));
            }
            else
            {
                test.Add((TItem)(IItem)(new BaseItem<E_Item>((E_Item)itemData)));
            }
        }

        return test;
    }
}
