using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.Character.Attributes
{
    [CreateAssetMenu(menuName = "RPG/StatBlockDefinitionDefinition")]
    public class StatBlockDefinition : ScriptableObject
    {
        public List<AttributeDefinition> attributes;
    }

    [Serializable]
    public class AttributeDefinition
    {
        public string attributeName;
        public string type;
        public int value;
        public int currentValue;
        public string relationship;
    }
}