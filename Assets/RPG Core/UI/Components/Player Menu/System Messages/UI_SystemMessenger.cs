using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

[RequireComponent(typeof(UIDocument))]
public class SystemMessenger : MonoBehaviour, ISystemMessenger
{
    private VisualElement root;
    private Queue<Label> _systemMessages = new Queue<Label>();

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement.Query<VisualElement>("system-messages").First().parent;
        var existingElements = root.Query<Label>().ToList();
        foreach (var label in existingElements)
        {
            StartCoroutine(DestoryAfter(3f));
            _systemMessages.Enqueue(label);
        }
    }

    void DestroyNext()
    {
        if (_systemMessages.Count == 0) return;
        var label = _systemMessages.Dequeue();
        root.Remove(label);
        label.Clear();
    }

    IEnumerator DestoryAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DestroyNext();
    }

    public void ShowMessage(string text,Color textColor,bool ignoreRepeats = false)
    {
        if (_systemMessages.Count > 2) DestroyNext();

        var label = new Label();
        label.text = text;
        label.style.color = textColor;
        label.AddToClassList("system-message");
        _systemMessages.Enqueue(label);
        root.Add(label);
        StartCoroutine(DestoryAfter(3f));
    }
}
