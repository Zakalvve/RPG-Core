using RotaryHeart.Lib.SerializableDictionary;
using RPG.Core.Character.Attributes;
using RPG.Core.Events;
using System;

namespace RPG.Core.SerializableDictionary
{
    [Serializable]
    public class SerializableDictionaryStringRPGEvent : SerializableDictionaryBase<string, RPGEvent<RPGEventArgs>> { }

    //[Serializable]
    public class SerializableDictionaryStringAttribute : SerializableDictionaryBase<string,IRPGAttribute> { }

}