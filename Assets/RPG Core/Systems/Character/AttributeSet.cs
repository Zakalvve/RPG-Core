using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using System.Linq;
using RPG.Core.SerializableDictionary;
using RPG.Core.Events;

namespace RPG.Core.Character.Attributes
{
    public class AttributeSet
    {
        #region Creating new Attribute Set
        public static bool AttributesFromCharacter(BGId characterID,out AttributeSet attributes)
        {
            attributes = null;
            var characterEntity = E_Character.GetEntity(characterID);
            if (characterEntity == null) return false;
            attributes = AttributesFromCharacter(characterEntity);
            return true;
        }
        public static AttributeSet AttributesFromCharacter(E_Character character)
        {
            E_StatBlock statsData = character.f_stats;
            if (statsData == null)
            {
                statsData = E_StatBlock.NewEntity();
                statsData.f_name = character.f_name + "_Stats";
                character.f_stats = statsData;
            }

            List<IAttributeData> attributeData;
            //we need to ensure the entity data is correct before proceeding
            (statsData.f_stats, attributeData) = CreateAttributesFromDefinitions(character.f_name,statsData.f_stats);

            (SerializableDictionaryStringAttribute stats, Dictionary<Type,Dictionary<string,IRPGAttribute>> statsByType) = CreateAttributes(attributeData);
            FormRelationships(stats);
            return new AttributeSet(stats,statsByType);
        }
        private static (List<E_v_attribute>, List<IAttributeData>) CreateAttributesFromDefinitions(string owningEntity, List<E_v_attribute> input = null)
        {
            input = input ?? new List<E_v_attribute>();

            var output = new List<IAttributeData>();
            //cycle through all definitions
            E_AttributeDefinition.ForEachEntity(definition =>
            {
                if (input.Exists(att => att.f_name == $"{owningEntity}_{definition.f_name}"))
                {
                    var att = input.Find(att => att.f_name == $"{owningEntity}_{definition.f_name}");
                    output.Add(CreateAttributeDataFromDbAttribute(att, definition));
                }
                else
                {
                    if (CreateAttributeFromDefinition(owningEntity,definition, out E_v_attribute newAttribute))
                    {
                        input.Add(newAttribute);
                        output.Add(CreateAttributeDataFromDbAttribute(newAttribute,definition));
                    } else
                    {
                        output.Add(new AttributeDataDerrived(definition));
                    }
                }
            });

            return (input,output);
        }
        private static IAttributeData CreateAttributeDataFromDbAttribute(E_v_attribute attribute, E_AttributeDefinition definition)
        {
            if (attribute is E_Vital)
            {
                return new VitalDataDb((E_Vital)attribute,definition);
            } 
            else if (attribute is E_Attribute)
            {
                return new AttributeDataDb((E_Attribute)attribute,definition);
            }
            throw new Exception("Unsure of attribute type");
        }
        private static bool CreateAttributeFromDefinition(string owningEntity, E_AttributeDefinition definition, out E_v_attribute output)
        {
            output = null;
            if (definition.f_type == "vital" || definition.f_type == "resource")
            {
                var vital = E_Vital.NewEntity();
                vital.f_definition = definition;
                vital.f_name = $"{owningEntity}_{definition.f_name}";
                vital.f_type = definition.f_type;
                vital.f_baseValue = definition.f_startingValue;
                vital.f_value = definition.f_startingValue;
                vital.f_currentValue = -1;
                output = vital;
                return true;
            }
            else if (definition.f_type == "level" || definition.f_type == "primary")
            {
                var attribute = E_Attribute.NewEntity();
                attribute.f_definition = definition;
                attribute.f_name = $"{owningEntity}_{definition.f_name}";
                attribute.f_type = definition.f_type;
                attribute.f_baseValue = definition.f_startingValue;
                attribute.f_value = definition.f_startingValue;
                output = attribute;
                return true;
            }

            return false;
        }
        private static (SerializableDictionaryStringAttribute, Dictionary<Type,Dictionary<string,IRPGAttribute>>) CreateAttributes(List<IAttributeData> input)
        {
            SerializableDictionaryStringAttribute stats = new SerializableDictionaryStringAttribute();
            Dictionary<Type,Dictionary<string,IRPGAttribute>> statsByType = new Dictionary<Type,Dictionary<string,IRPGAttribute>>();

            foreach (var data in input)
            {
                //(E_v_attribute attData, E_AttributeDefinition def) = data;
                if (data is IVitalData)
                {
                    bool isResource = data.Type == "resource";
                    RPGVital att = new RPGVital((IVitalData)data,isResource: isResource);
                    Type t = typeof(RPGVital);
                    if (!statsByType.ContainsKey(t))
                    {
                        statsByType.Add(t,new Dictionary<string,IRPGAttribute>());
                    }
                    statsByType[t].Add(data.Name,att);

                    stats.Add(data.Name, att);
                }
                else
                {
                    RPGAttribute att = new RPGAttribute((IAttributeData)data);
                    Type t = typeof(RPGAttribute);
                    if (!statsByType.ContainsKey(t))
                    {
                        statsByType.Add(t,new Dictionary<string,IRPGAttribute>());
                    }
                    statsByType[t].Add(data.Name,att);

                    stats.Add(data.Name,att);
                }
            }
            return (stats,statsByType);
        }
        private static void FormRelationships(SerializableDictionaryStringAttribute attributes)
        {
            foreach (var att in attributes)
            {
                if (!String.IsNullOrEmpty(att.Value.Relationship))
                {
                    (List<IRPGAttribute> sources, string formula) = ExtractRelationship(att.Value.Relationship, attributes);
                    att.Value.FormRelationship(sources,formula);
                }
            }

            foreach (var att in attributes)
            {
                att.Value.Sync();
            }
        }
        private static (List<IRPGAttribute>, string) ExtractRelationship(string relationship,SerializableDictionaryStringAttribute attributes)
        {
            List<IRPGAttribute> sources = new List<IRPGAttribute>();
            string formula = relationship;

            var parameters = relationship.Split('[',']').Where((item,index) => index % 2 != 0).ToList();
            for (int i = 0; i < parameters.Count; i++)
            {
                if (TryGetAttribute(parameters[i].ToLower(), attributes,out IRPGAttribute source))
                {
                    formula = formula.Replace(parameters[i],i.ToString());
                    sources.Add(source);
                }
                else
                {
                    throw new KeyNotFoundException($"Could not find the key {parameters[i].ToLower()} in stat dictionary");
                }
            }

            return (sources, formula);
        }
        private static bool TryGetAttribute(string name,SerializableDictionaryStringAttribute attributes,out IRPGAttribute attribute)
        {
            attribute = null;
            if (attributes.TryGetValue(name,out IRPGAttribute foundAttribute))
            {
                attribute = foundAttribute;
                return true;
            }
            return false;
        }
        #endregion

        public int Level
        {
            get
            {
                return (int)_level.Value;
            }
        }
        public RPGHealth Health
        {
            get 
            {
                return _health;
            }
        }
        public IRPGVital Essence
        {
            get
            {
                return _essence;
            }
        }

        private AttributeSet(SerializableDictionaryStringAttribute attributes,Dictionary<Type,Dictionary<string,IRPGAttribute>> attributesByClassType)
        {
            _attributes = attributes;
            _attributesByClassType = attributesByClassType;
            _health = new RPGHealth();

            if (TryGetAttribute("level",out RPGAttribute level))
            {
                _level = level;
            }

            if (TryGetAttribute("health", out RPGVital health))
            {
                _health.AssignHealth(health);
            }
            if (TryGetAttribute("vigor", out RPGVital vigor)){
                _health.AssignVigor(vigor);
            }

            if (TryGetAttribute("essence",out RPGVital essence))
            {
                _essence = essence;
            }
        }

        private RPGAttribute _level;
        private RPGHealth _health;
        private RPGVital _essence;

        private SerializableDictionaryStringAttribute _attributes;
        private Dictionary<Type,Dictionary<string,IRPGAttribute>> _attributesByClassType;
        public bool TryGetAttribute<T>(string name,out T attribute) where T : class, IRPGAttribute
        {
            attribute = null;
            if (_attributesByClassType.TryGetValue(typeof(T),out Dictionary<string,IRPGAttribute> TDict))
            {
                if (TDict.TryGetValue(name,out IRPGAttribute foundAttribute))
                {
                    attribute = (T)foundAttribute;
                    return true;
                }
            }
            return false;
        }
        public bool TryGetAttribute(string name, out IRPGAttribute attribute)
        {
            attribute = null;

            if (_attributes.TryGetValue(name,out attribute))
            {
                return true;
            }

            return false;
        }
        public bool TryGetVital(string name, out IRPGVital vital)
        {
            if (TryGetAttribute(name, out vital)) {
                return true;
            }
            return false;
        }

        #region Attribute Classes
        private abstract class BaseRPGAttribute<T> : IRPGAttribute where T : IAttributeData
        {
            public string Name
            {
                get
                {
                    return _data.Name;
                }
            }
            public string Type
            {
                get
                {
                    return _data.Type;
                }
            }
            public float Value
            {
                get
                {
                    return _data.Value;
                }
                private set
                {
                    _data.Value = value;
                }
            }
            public float BaseValue
            {
                get
                {
                    return _data.BaseValue;
                }
                set
                {
                    if (value < 0) value = 0;
                    _data.BaseValue = value;
                    CalculateValue();
                }
            }
            public string Relationship
            {
                get
                {
                    return _data.Relationship;
                }
            }

            public virtual void Bind(Action<IRPGAttribute> handleObjectChanged)
            {
                ObjectChanged += handleObjectChanged;
            }
            public virtual void Unbind(Action<IRPGAttribute> handleObjectChanged)
            {
                ObjectChanged -= handleObjectChanged;
            }
            public virtual void Sync()
            {
                CalculateValue();
            }

            public void FormRelationship(List<IRPGAttribute> sources,string formula)
            {
                var ship = new AttributeRelationship(sources,this,formula);
                ship.Form();
                _relationship = ship;
                CalculateValue();
            }
            public void AddModifier(IAttributeModifier modifier)
            {
                _modifiers.AddModifier(modifier);
                CalculateValue();
            }
            public void AddModifiers(IEnumerable<IAttributeModifier> modifiers)
            {
                _modifiers.AddModifiers(modifiers);
                CalculateValue();
            }
            public void RemoveModifiersById(string sourceId)
            {
                _modifiers.RemoveModifiersById(sourceId);
                CalculateValue();
            }

            public BaseRPGAttribute(T data)
            {
                _data = data;
                _data.OnValueChanged += Sync;
            }

            protected T _data;
            protected event Action<IRPGAttribute> ObjectChanged;
            private AttributeModifierCollection _modifiers = new AttributeModifierCollection();
            private AttributeRelationship _relationship;
            private float _modifierValue = 0f;
            private float _relationshipValue = 0f;
            protected virtual void CalculateValue()
            {
                _relationshipValue = _relationship?.Evaluate() ?? 0f;
                _modifierValue = _modifiers?.EvaluateAllModifiersOnValue(BaseValue + _relationshipValue) ?? 0f;
                Value = BaseValue + _relationshipValue + _modifierValue;
                ObjectChanged?.Invoke(this);
            }

            #region Classes
            private class AttributeRelationship
            {
                private List<IRPGAttribute> _sources;
                private BaseRPGAttribute<T> _target;
                private NCalc.Expression _expression;
                public AttributeRelationship(IRPGAttribute source,BaseRPGAttribute<T> target,string formula) : this(new List<IRPGAttribute>() { source },target,formula) { }
                public AttributeRelationship(List<IRPGAttribute> sources,BaseRPGAttribute<T> target,string formula)
                {
                    _sources = sources;
                    _target = target;
                    _expression = new NCalc.Expression(formula);
                    BuildExpression();
                }
                public void Form()
                {
                    _sources.ForEach(source => source.Bind(OnSourceValueChange));
                }
                public void Break()
                {
                    _sources.ForEach(source => source.Unbind(OnSourceValueChange));
                }
                public float Evaluate()
                {
                    return (float)_expression.Evaluate();
                }
                private void BuildExpression()
                {
                    for (int i = 0; i < _sources.Count; i++)
                    {
                        _expression.Parameters[i.ToString()] = _sources[i].Value;
                    }
                }
                private void OnSourceValueChange(IRPGAttribute source)
                {
                    BuildExpression();
                    _target.CalculateValue();
                }
            }
            private class AttributeModifierCollection
            {
                public int Count => _addMods.Count + _percentAddMods.Count + _percentMultMods.Count;

                private ICollection<IAttributeModifier> _addMods = new List<IAttributeModifier>();
                private ICollection<IAttributeModifier> _percentAddMods = new List<IAttributeModifier>();
                private ICollection<IAttributeModifier> _percentMultMods = new List<IAttributeModifier>();

                public void AddModifier(IAttributeModifier modifier)
                {
                    switch (modifier.Operation)
                    {
                        case AttributeModifierOperation.Add:
                            _addMods.Add(modifier);
                            break;
                        case AttributeModifierOperation.PercentAdd:
                            _percentAddMods.Add(modifier);
                            break;
                        case AttributeModifierOperation.PercentMult:
                            _percentMultMods.Add(modifier);
                            break;
                    }
                }
                public void AddModifiers(IEnumerable<IAttributeModifier> modifiers)
                {
                    foreach (var modifier in modifiers)
                    {
                        AddModifier(modifier);
                    }
                }
                public void RemoveModifiersById(string sourceId)
                {
                    foreach (var mod in _addMods)
                    {
                        if (mod.SourceId == sourceId)
                        {
                            _addMods.Remove(mod);
                        }
                    }
                    foreach (var mod in _percentAddMods)
                    {
                        if (mod.SourceId == sourceId)
                        {
                            _percentAddMods.Remove(mod);
                        }
                    }
                    foreach (var mod in _percentMultMods)
                    {
                        if (mod.SourceId == sourceId)
                        {
                            _percentMultMods.Remove(mod);
                        }
                    }
                }
                public float EvaluateAllModifiersOnValue(float value)
                {
                    if (Count <= 0) return 0f;

                    float workingValue = value;
                    float percentAddValue = 0f;

                    foreach (var add in _addMods)
                    {
                        workingValue += add.Value;
                    }
                    foreach (var percentAdd in _percentAddMods)
                    {
                        percentAddValue += percentAdd.Value;
                    }
                    if (percentAddValue > 0)
                    {
                        workingValue *= 1 + percentAddValue;
                    }
                    foreach (var percentMult in _percentMultMods)
                    {
                        workingValue *= percentMult.Value;
                    }

                    return workingValue - value;
                }
            }
            #endregion
        }
        private class RPGAttribute : BaseRPGAttribute<IAttributeData>
        {
            public RPGAttribute(IAttributeData data) : base(data) { }
        }
        private class RPGVital : BaseRPGAttribute<IVitalData>, IRPGVital
        {
            private bool _isResource;

            public float CurrentValue
            {
                get
                {
                    return _data.CurrentValue;
                }
                private set
                {
                    _data.CurrentValue = Mathf.Clamp(value,0,Value);
                    CalculateValue();
                }
            }
            public override void Sync()
            {
                base.Sync();
                if (CurrentValue < 0) CurrentValue = Value;
            }
            public virtual void ChangeCurrentValue(float amount)
            {
                if (CurrentValue + amount <= 0 && _isResource) throw new Exception($"You don't have enough ${Name} to do that.");
                CurrentValue += amount;
            }

            private event Action<IRPGVital> VitalChanged;

            public void Bind(Action<IRPGVital> handleObjectChanged)
            {
                VitalChanged += handleObjectChanged;
            }

            public void Unbind(Action<IRPGVital> handleObjectChanged)
            {
                VitalChanged += handleObjectChanged;
            }

            protected override void CalculateValue()
            {
                base.CalculateValue();
                VitalChanged?.Invoke(this);
            }

            public RPGVital(IVitalData vital,/*float currentValue = -1,*/bool isResource = false) : base(vital)
            {
                //bypass property and set current value to -1 or the current value stored in data base
                //later if the value is still -1 we assume that this is a new vital and set its current value equal to its value
                //we can't do that now as its calculated value is dependant on other attributes first being defined
                //_data.CurrentValue = currentValue;
                _isResource = isResource;
            }
        }
        public class RPGHealth
        {
            public IRPGVital Health { get { return _health; } }
            public IRPGVital Vigor { get { return _vigor; } }

            [SerializeField]
            private IRPGVital _health;
            [SerializeField]
            private IRPGVital _vigor;

            event Action OnHealthDepleted;
            event Action OnVigourDepleted;

            public void AssignHealth(IRPGVital health)
            {
                _health = health;
            }

            public void AssignVigor(IRPGVital vigor)
            {
                _vigor = vigor;
            }

            public void ChangeHealth(float amount)
            {
                _health.ChangeCurrentValue(amount);
                _vigor.ChangeCurrentValue(amount);

                if (_health.CurrentValue <= 0) OnHealthDepleted?.Invoke();
                if (_vigor.CurrentValue <= 0) OnVigourDepleted?.Invoke();
            }
        }
        public class AttributeModifier : IAttributeModifier
        {
            public string SourceId { get; private set; }
            public float Value { get; private set; }
            public AttributeModifierOperation Operation { get; private set; }

            public AttributeModifier(string sourceId,float value,AttributeModifierOperation operation)
            {
                SourceId = sourceId;
                Value = value;
                Operation = operation;
            }
        }
        public enum AttributeModifierOperation
        {
            Add,
            PercentAdd,
            PercentMult
        }
        public interface IAttributeData
        {
            string Name { get; }
            string Type { get; }
            float Value { get; set; }
            float BaseValue { get; set; }
            string Relationship { get; }
            event Action OnValueChanged;
        }
        public interface IVitalData : IAttributeData
        {
            float CurrentValue { get; set; }
        }
        public class AttributeDataDerrived : IAttributeData
        {
            private float _value;
            public float Value { get => _value; set => _value = value; }
            private float _baseValue;
            public float BaseValue { get => _baseValue; set => _baseValue = value; }

            //if the attribute definition is null the attribute is assumed to have no relationships
            //this is used for simple values like current health etc
            private E_AttributeDefinition _definition;
            public string Relationship => _definition.f_relationship;
            public string Type => _definition.f_type;
            public string Name => _definition.f_name;

            public event Action OnValueChanged;

            public AttributeDataDerrived(E_AttributeDefinition definition)
            {
                _definition = definition;
            }
        }
        public abstract class AttributeDataDb<T> : IAttributeData where T : E_v_attribute
        {
            protected T _data;
            public string Name => _definition.f_name;
            public float Value { get => _data.f_value; set => _data.f_value = value; }
            public float BaseValue { get => _data.f_baseValue; set => _data.f_baseValue = value; }

            private E_AttributeDefinition _definition;
            public string Type => _definition.f_type;
            public string Relationship => _definition.f_relationship;

            public event Action OnValueChanged;

            public AttributeDataDb(T data, E_AttributeDefinition definition)
            {
                _data = data;
                _definition = definition;

                //set up listening for db changes
                EventEmitter.OnReset += RemoveListeners;
                _data.Meta.AddEntityUpdatedListener(_data.Id,OnAttributeChanged);
            }

            protected void ValueChanged()
            {
                OnValueChanged?.Invoke();
            }
            
            protected virtual void RemoveListeners()
            {
                _data.Meta.RemoveEntityUpdatedListener(_data.Id,OnAttributeChanged);
            }
            ~AttributeDataDb()
            {
                RemoveListeners();
            }

            public void OnAttributeChanged(object sender,BGEventArgsEntityUpdated args)
            {
                if (args.FieldId == _data.Meta.GetFieldId("baseValue"))
                {
                    ValueChanged();
                }
            }
        }
        public class AttributeDataDb : AttributeDataDb<E_Attribute>
        {
            public AttributeDataDb(E_Attribute data,E_AttributeDefinition definition) : base(data,definition) { }
        }
        public class VitalDataDb : AttributeDataDb<E_Vital>, IVitalData
        {
            public VitalDataDb(E_Vital data,E_AttributeDefinition definition) : base(data,definition) 
            {
                EventEmitter.OnReset += RemoveListeners;
                _data.Meta.AddEntityUpdatedListener(_data.Id,OnVitaChanged);
            }

            public float CurrentValue { get => _data.f_currentValue; set => _data.f_currentValue = value; }

            ~VitalDataDb()
            {
                RemoveListeners();
            }

            protected override void RemoveListeners()
            {
                base.RemoveListeners();
                _data.Meta.RemoveEntityUpdatedListener(_data.Id,OnVitaChanged);
            }

            public void OnVitaChanged(object sender,BGEventArgsEntityUpdated args)
            {
                if (args.FieldId == _data.Meta.GetFieldId("currentValue"))
                {
                    ValueChanged();
                }
            }
        }

        #endregion
    }
}