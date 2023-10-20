using Item;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using Zenject;

public class TooltipController
{
    private VisualElement _root;
    public VisualTreeAsset itemTooltipTemplate;
    public VisualElement itemTooltip;

    [Inject]
    public TooltipController(UIDocument UI)
    {
        _root = UI.rootVisualElement;
        itemTooltipTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RPG Core/UI/Components/Tooltips/ItemTooltip.uxml");
        itemTooltip = itemTooltipTemplate.CloneTree().Children().ElementAt(0);
        itemTooltip.style.display = DisplayStyle.None;
    }

    public void ShowTooltip(VisualElement reference, IItem item)
    {
        itemTooltip.Q<Label>("name").text = item.f_name;
        itemTooltip.Q<Label>("type").text = item.f_Type;
        itemTooltip.Q<Label>("description").text = item.f_Description;
        itemTooltip.style.display = DisplayStyle.Flex;
        itemTooltip.RegisterCallback<GeometryChangedEvent,(VisualElement, VisualElement)>(AlignTooltip, (reference,itemTooltip));
        _root.Add(itemTooltip);
    }

    public void HideTooltip()
    {
        itemTooltip.style.display = DisplayStyle.None;
        itemTooltip.UnregisterCallback<GeometryChangedEvent,(VisualElement, VisualElement)>(AlignTooltip);
    }

    private void AlignTooltip(GeometryChangedEvent e, (VisualElement reference, VisualElement tooltip) input)
    {
        if (input.reference.worldBound.xMax + 10 + input.tooltip.layout.width > _root.layout.width)
        {
            //item spills out of the screen so re poisition on left side
            input.tooltip.style.left = input.reference.worldBound.xMin - 10 - input.tooltip.layout.width;
        } else
        {
            input.tooltip.style.left = input.reference.worldBound.xMax + 10;
        }

        if (input.reference.worldBound.yMin + input.tooltip.layout.height > _root.layout.height)
        {
            input.tooltip.style.top = input.reference.worldBound.yMax - input.tooltip.layout.height;
        } else
        {
            input.tooltip.style.top = input.reference.worldBound.yMin;
        }
    }

    ~TooltipController()
    {
        itemTooltip.UnregisterCallback<GeometryChangedEvent,(VisualElement, VisualElement)>(AlignTooltip);
    }
}
