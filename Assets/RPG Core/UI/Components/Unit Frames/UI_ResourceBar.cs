using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Core.UI.Components.UnitFrames
{
    public class UI_ResourceBar : VisualElement, INotifyValueChanged<int>
    {

        public class UXMLFactory : UxmlFactory<UI_ResourceBar,UxmlTraits> { }
        public int width { get; set; }
        public int height { get; set; }
        public Color fillColor { get; set; }
        public Color backgroundColor { get; set; }

        public int value
        {
            get
            {
                m_currentValue = Mathf.Clamp(m_currentValue,0,maxValue);
                return m_currentValue;
            }
            set
            {
                if (EqualityComparer<int>.Default.Equals(m_currentValue,value))
                    return;

                if (panel != null)
                {
                    using (ChangeEvent<int> pooled = ChangeEvent<int>.GetPooled(m_currentValue,value))
                    {
                        pooled.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(pooled);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                }
            }
        }

        private int m_currentValue;

        public float valueProportion => (float)m_currentValue / maxValue;


        private int _maxValue = 100;
        public int maxValue 
        { 
            get 
            { 
                return _maxValue; 
            } 
            set 
            {
                _maxValue = value;
                UpdateDisplay();
            } 
        }

        public enum FillType
        {
            Horizontal,
            Vertical
        }

        public FillType fillType { get; set; }

        public enum TextDisplayType
        {
            Raw,
            Percent
        }
        public TextDisplayType textDisplayType { get; set; }

        private VisualElement _root;
        private VisualElement background;
        private VisualElement foreground;
        private Label resourceText;

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription m_width = new UxmlIntAttributeDescription() { name = "width",defaultValue = 100 };
            UxmlIntAttributeDescription m_height = new UxmlIntAttributeDescription() { name = "height",defaultValue = 100 };
            UxmlIntAttributeDescription m_value = new UxmlIntAttributeDescription() { name = "value",defaultValue = 100 };
            UxmlIntAttributeDescription m_maxValue = new UxmlIntAttributeDescription() { name = "max-value",defaultValue = 100 };
            UxmlFloatAttributeDescription m_textSize = new UxmlFloatAttributeDescription() { name = "text-size",defaultValue = 2.2f };
            UxmlEnumAttributeDescription<FillType> m_fillType = new UxmlEnumAttributeDescription<FillType>() { name = "fill-type",defaultValue = 0 };
            UxmlEnumAttributeDescription<TextDisplayType> m_textDisplayType = new UxmlEnumAttributeDescription<TextDisplayType>() { name = "text-display-type",defaultValue = 0 };
            UxmlColorAttributeDescription m_fillColor = new UxmlColorAttributeDescription() { name = "fill-color",defaultValue = Color.red };
            UxmlColorAttributeDescription m_backgroundColor = new UxmlColorAttributeDescription() { name = "background-color",defaultValue = Color.black };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement visualElement,IUxmlAttributes bag,CreationContext context)
            {
                base.Init(visualElement,bag,context);

                var element = visualElement as UI_ResourceBar;
                element.width = m_width.GetValueFromBag(bag,context);
                element.height = m_height.GetValueFromBag(bag,context);
                element._maxValue = m_maxValue.GetValueFromBag(bag,context);
                element.m_currentValue = m_value.GetValueFromBag(bag,context);
                element.fillType = m_fillType.GetValueFromBag(bag,context);
                element.textDisplayType = m_textDisplayType.GetValueFromBag(bag, context);
                element.fillColor = m_fillColor.GetValueFromBag(bag,context);
                element.backgroundColor = m_backgroundColor.GetValueFromBag(bag,context);

                element.Clear();

                VisualTreeAsset component = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RPG Core/UI/Components/Unit Frames/ResourceBar.uxml");
                VisualElement resourceBar = component.Instantiate();

                element._root = resourceBar.Q<VisualElement>("resource-bar-root");
                element.background = resourceBar.Q<VisualElement>("background");
                element.foreground = resourceBar.Q<VisualElement>("foreground");
                element.resourceText = resourceBar.Q<Label>("resource-text");

                element.Add(resourceBar);

                if (element.fillType == FillType.Horizontal)
                {
                    element.style.width = Length.Percent(element.width);
                    element._root.style.width = element.layout.width;
                }
                else
                {
                    //weird stuff going on here some unity bug is causing problems
                    //https://forum.unity.com/threads/bug-in-ui-builder-nullref-when-building-custom-element-tree-from-uxml-unity-2021-2-6f1.1217169/
                    //element._root.style.width = Length.Percent(100);
                    //element._root.style.height = element.style.height;
                    //element.style.width = Length.Percent(element.height / 6);
                    //element.style.height = Length.Percent(element.height);
                }

                resourceBar.style.flexGrow = 1;

                element.foreground.style.backgroundColor = element.fillColor;

                element.RegisterValueChangedCallback(element.UpdateResourceValue);
                element.RegisterCallback<AttachToPanelEvent>((e) => element.UpdateDisplay());
                element.UpdateDisplay();
            }
        }

        private void UpdateResourceValue(ChangeEvent<int> e)
        {
            UpdateDisplay();
        }

        public void SetValueWithoutNotify(int newValue)
        {
            m_currentValue = newValue;
        }

        private void UpdateDisplay()
        {
            if (fillType == FillType.Horizontal)
            {
                foreground.style.scale = new Scale(new Vector3(valueProportion,1f,1f));
            }
            else
            {
                foreground.style.scale = new Scale(new Vector3(1f,valueProportion,1f));
            }

            if (textDisplayType == TextDisplayType.Raw)
            {
                resourceText.text = $"{value}/{maxValue}";
            } else
            {
                resourceText.text = $"{valueProportion*100}%";
            }
        }
    }
}