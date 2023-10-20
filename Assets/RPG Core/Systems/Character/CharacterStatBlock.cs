using RPG.Core.Character;
using UnityEngine;

[RequireComponent(typeof(PartyCharacter))]
public class CharacterStatBlock : M_StatBlock
{
    private new void Awake()
    {
        var character = GetComponent<PartyCharacter>();
        Entity = character.f_stats;
    }
}
