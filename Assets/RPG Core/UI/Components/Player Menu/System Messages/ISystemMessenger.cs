using UnityEngine;

public interface ISystemMessenger
{
    void ShowMessage(string text,Color textColor,bool ignoreRepeats = false);
}