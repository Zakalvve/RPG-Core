using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using RPG.Core.UI.Components.UnitFrames;

public class UI_InventoryManager : MonoBehaviour
{
    public void Initialize()
    {
     
    }

    private void OnValidate()
    {
        
    }

    public static (string displayId, string slotIndex) SplitID(string id)
    {
        var ids = id.Split('@');
        return (ids[0], ids[1]);
    }
    public static string FormID(string displayId,string slotindex)
    {
        return $"{displayId}@{slotindex}";
    }
}