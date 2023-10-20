using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
[CreateAssetMenu(menuName = "My Assets/Test Sprites List")]
public class TestSpritesList : ScriptableObject
{
    [SerializeField]
    public List<Sprite> Sprites = new List<Sprite>();
}
