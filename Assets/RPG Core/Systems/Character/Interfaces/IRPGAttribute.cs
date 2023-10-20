using InventorySystem;
using System;
using System.Collections.Generic;

namespace RPG.Core.Character.Attributes
{
    public interface IRPGAttribute : IBindableObject<IRPGAttribute>
    {
        string Name { get; set; }
        string Type { get; }
        float Value { get; }
        float BaseValue { get; set; }
        string Relationship { get; }
        void Sync();
        void FormRelationship(List<IRPGAttribute> sources,string formula);
        void AddModifier(IAttributeModifier modifier);
        void AddModifiers(IEnumerable<IAttributeModifier> modifiers);
        void RemoveModifiersById(string sourceId);
    }

    public interface IAttributeModifier
    {
        string SourceId { get; }
        float Value { get; }
        StatBlock.AttributeModifierOperation Operation { get; }
    }

    public interface IRPGVital : IRPGAttribute, IBindableObject<IRPGVital>
    {
        float CurrentValue { get; }
        void ChangeCurrentValue(float amount);
    }
}