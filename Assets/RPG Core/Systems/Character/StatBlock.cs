using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using Ardalis.SmartEnum;
using System.Linq;
using RPG.Core.SerializableDictionary;
using RPG.Core.Events;

namespace RPG.Core.Character.Attributes
{
    public class StatBlock
    {
        #region Creating New Stat Blocks

        public static bool StatBlockFromCharacter(BGId characterID,out StatBlock stats)
        {
            stats = null;
            var characterEntity = E_Character.GetEntity(characterID);
            if (characterEntity == null) return false;
            stats = StatBlockFromCharacter(characterEntity);
            return true;
        }
        public static StatBlock StatBlockFromCharacter(E_Character character)
        {
            E_StatBlock statsData = character.f_stats;
            if (statsData == null)
            {
                statsData = E_StatBlock.NewEntity();
                statsData.f_name = character.f_name + "_Stats";
                character.f_stats = statsData;
            }

            if (!ValidateAttributeList(statsData.f_stats))
            {
                //we need to ensure the entity data is correct before proceeding
                statsData.f_stats = CreateAttributesFromDefinitions(character.f_name, statsData.f_stats);
            }

            (SerializableDictionaryStringAttribute stats, Dictionary<Type,Dictionary<string,IRPGAttribute>> statsByType) = CreateAttributes(ExtractDefinitions(statsData.f_stats));
            FormRelationships(stats);
            return new StatBlock(stats,statsByType);
        }
        private static bool ValidateAttributeList(List<E_v_attribute> data)
        {
            return data == null ? false : data.Count == E_AttributeDefinition.CountEntities;
        }
        private static List<E_v_attribute> CreateAttributesFromDefinitions(string owningEntity, List<E_v_attribute> input = null)
        {
            var output = input;
            if (output == null) output = new List<E_v_attribute>();
            //cycle through all definitions
            E_AttributeDefinition.ForEachEntity(definition =>
            {
                if (output.Exists(att => att.f_name == definition.f_name)) return;
                output.Add(CreateAttributeFromDefinition(owningEntity,definition));
            });

            return output;
        }
        private static E_v_attribute CreateAttributeFromDefinition(string owningEntity, E_AttributeDefinition definition)
        {
            if (definition.f_type == "vital" || definition.f_type == "resource")
            {
                var output = E_Vital.NewEntity();
                output.f_definition = definition;
                output.f_name = $"{owningEntity}_{definition.f_name}";
                output.f_type = definition.f_type;
                output.f_baseValue = definition.f_startingValue;
                output.f_value = definition.f_startingValue;
                output.f_currentValue = -1;
                return output;
            }
            else
            {
                var output = E_Attribute.NewEntity();
                output.f_definition = definition;
                output.f_name = $"{owningEntity}_{definition.f_name}";
                output.f_type = definition.f_type;
                output.f_baseValue = definition.f_startingValue;
                output.f_value = definition.f_startingValue;
                return output;
            }
        }
        private static List<(E_v_attribute, E_AttributeDefinition)> ExtractDefinitions(List<E_v_attribute> input)
        {
            var output = new List<(E_v_attribute, E_AttributeDefinition)>();
            foreach (var item in input)
            {
                if (item is E_Attribute)
                {
                    output.Add((item, ((E_Attribute)item).f_definition));
                }
                if (item is E_Vital)
                {
                    output.Add((item, ((E_Vital)item).f_definition));
                }
            }
            return output;
        }
        private static (SerializableDictionaryStringAttribute, Dictionary<Type,Dictionary<string,IRPGAttribute>>) CreateAttributes(List<(E_v_attribute, E_AttributeDefinition)> input)
        {
            SerializableDictionaryStringAttribute stats = new SerializableDictionaryStringAttribute();
            Dictionary<Type,Dictionary<string,IRPGAttribute>> statsByType = new Dictionary<Type,Dictionary<string,IRPGAttribute>>();

            foreach (var data in input)
            {
                (E_v_attribute attData, E_AttributeDefinition def) = data;
                if (def.f_type.ToLower() == "vital")
                {
                    RPGVital att = new RPGVital((E_Vital)attData,def, ((E_Vital)attData).f_currentValue);
                    Type t = typeof(RPGVital);
                    if (!statsByType.ContainsKey(t))
                    {
                        statsByType.Add(t,new Dictionary<string,IRPGAttribute>());
                    }
                    statsByType[t].Add(def.f_name,att);

                    stats.Add(def.f_name, att);
                }
                else if (def.f_type.ToLower() == "resource")
                {
                    RPGResource att = new RPGResource((E_Vital)attData,def,((E_Vital)attData).f_currentValue);
                    Type t = typeof(RPGResource);
                    if (!statsByType.ContainsKey(t))
                    {
                        statsByType.Add(t,new Dictionary<string,IRPGAttribute>());
                    }
                    statsByType[t].Add(def.f_name,att);

                    stats.Add(def.f_name,att);
                }
                else
                {
                    RPGAttribute att = new RPGAttribute((E_Attribute)attData,def);
                    Type t = typeof(RPGAttribute);
                    if (!statsByType.ContainsKey(t))
                    {
                        statsByType.Add(t,new Dictionary<string,IRPGAttribute>());
                    }
                    statsByType[t].Add(def.f_name,att);

                    stats.Add(def.f_name,att);
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

        public RPGHealth Health
        {
            get 
            {
                return _health;
            }
        }
        public int Level
        {
            get
            {
                return (int)_level.Value;
            }
        }
        public IRPGVital Essence
        {
            get
            {
                return _essence;
            }
        }

        private StatBlock(SerializableDictionaryStringAttribute stats,Dictionary<Type,Dictionary<string,IRPGAttribute>> statsByClassType)
        {
            _stats = stats;
            _statsByClassType = statsByClassType;
            _health = new RPGHealth();
            if (TryGetAttribute<RPGVital>("health", out RPGVital health))
            {
                _health.AssignHealth(health);
            }
            if (TryGetAttribute<RPGVital>("vigor", out RPGVital vigor)){
                _health.AssignVigor(vigor);
            }

            if (TryGetAttribute<RPGAttribute>("level", out RPGAttribute level))
            {
                _level = level;
            }

            if (TryGetAttribute<RPGResource>("essence",out RPGResource essence))
            {
                _essence = essence;
            }
        }

        private RPGAttribute _level;
        private RPGHealth _health;
        private RPGResource _essence;

        private SerializableDictionaryStringAttribute _stats;
        private Dictionary<Type,Dictionary<string,IRPGAttribute>> _statsByClassType;
        public bool TryGetAttribute<T>(string name,out T attribute) where T : class, IRPGAttribute
        {
            attribute = null;
            if (_statsByClassType.TryGetValue(typeof(T),out Dictionary<string,IRPGAttribute> TDict))
            {
                if (TDict.TryGetValue(name,out IRPGAttribute foundAttribute))
                {
                    attribute = (T)foundAttribute;
                    return true;
                }
            }
            return false;
        }

        #region Attribute Classes
        public class AttributeModifierOperation : SmartEnum<AttributeModifierOperation>
        {
            private AttributeModifierOperation(string name,int value) : base(name,value) { }

            public static readonly AttributeModifierOperation Add = new AttributeModifierOperation(nameof(Add),100);
            public static readonly AttributeModifierOperation PercentAdd = new AttributeModifierOperation(nameof(PercentAdd),200);
            public static readonly AttributeModifierOperation PercentMultiply = new AttributeModifierOperation(nameof(PercentMultiply),300);
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
        private abstract class BaseRPGAttribute<T> : IRPGAttribute where T : E_v_attribute
        {
            public string Name
            {
                get
                {
                    return _data.Name;
                }
                set
                {
                    _data.Name = value;
                }
            }
            public string Type
            {
                get
                {
                    return _definition.f_type;
                }
            }
            public float Value
            {
                get
                {
                    return _data.f_value;
                }
                private set
                {
                    _data.f_value = value;
                }
            }
            public float BaseValue
            {
                get
                {
                    return _data.f_baseValue;
                }
                set
                {
                    if (value < 0) value = 0;
                    _data.f_baseValue = value;
                    CalculateValue();
                }
            }
            public string Relationship
            {
                get
                {
                    return _definition.f_relationship;
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
            }
            public void RemoveModifiersById(string sourceId)
            {
                _modifiers.RemoveModifiersById(sourceId);
            }

            public BaseRPGAttribute(T data,E_AttributeDefinition definition)
            {
                _data = data;
                _definition = definition;
                EventEmitter.OnReset += RemoveListeners;
                _data.Meta.AddEntityUpdatedListener(_data.Id,OnAttributeChanged);
            }

            protected virtual void RemoveListeners()
            {
                _data.Meta.RemoveEntityUpdatedListener(_data.Id,OnAttributeChanged);
            }
            ~BaseRPGAttribute()
            {
                RemoveListeners();
            }

            
            public void OnAttributeChanged(object sender, BGEventArgsEntityUpdated args)
            {
                if (args.FieldId == _data.Meta.GetFieldId("baseValue"))
                {
                    BaseValue = _data.f_baseValue;
                }
            }

            protected T _data;
            protected E_AttributeDefinition _definition;
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
                    modifier.Operation
                        .When(AttributeModifierOperation.Add).Then(() => _addMods.Add(modifier))
                        .When(AttributeModifierOperation.PercentAdd).Then(() => _percentAddMods.Add(modifier))
                        .When(AttributeModifierOperation.PercentMultiply).Then(() => _percentMultMods.Add(modifier));
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
        private class RPGAttribute : BaseRPGAttribute<E_Attribute>
        {
            public RPGAttribute(E_Attribute data,E_AttributeDefinition definition) : base(data,definition) { }

            #region Serialization
            [SerializeField]
            public float value;
            [SerializeField]
            public float baseValue;
            public virtual void OnBeforeSerialize()
            {
                value = Value;
                baseValue = BaseValue;
            }

            public virtual void OnAfterDeserialize()
            {
                BaseValue = baseValue;
            }
            #endregion
        }
        private class RPGVital : BaseRPGAttribute<E_Vital>, IRPGVital
        {
            public float CurrentValue
            {
                get
                {
                    return _data.f_currentValue;
                }
                private set
                {
                    _data.f_currentValue = Mathf.Clamp(value,0,Value);
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

            public RPGVital(E_Vital vital,E_AttributeDefinition definition,float currentValue = -1) : base(vital,definition)
            {
                //bypass property and set current value to -1 or the current value stored in data base
                //later if the value is still -1 we assume that this is a new vital and set its current value equal to its value
                //we can't do that now as its calculated value is dependant on other attributes first being defined
                _data.f_currentValue = currentValue;
                EventEmitter.OnReset += RemoveListeners;
                _data.Meta.AddEntityUpdatedListener(_data.Id,OnVitaChanged);
            }

            ~RPGVital()
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
                    CurrentValue = _data.f_currentValue;
                }
            }
        }
        private class RPGResource : RPGVital
        {
            public RPGResource(E_Vital vital,E_AttributeDefinition definition,float currentValue = -1) : base(vital,definition,currentValue) { }

            public override void ChangeCurrentValue(float amount)
            {
                if (CurrentValue + amount <= 0) throw new Exception($"You don't have enough ${Name} to do that.");
                base.ChangeCurrentValue(amount);
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
        #endregion
    }
}