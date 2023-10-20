using Assets.RPG_Core.Core;
using System;
using System.Collections.Generic;

namespace RPG.Core
{
    public class InstanceIndexer<T> where T : InstanceIndexer<T>, IIDable
    {
        public string ID { get; private set; }
        public static readonly Dictionary<string,T> Instances = new Dictionary<string,T>();
        public InstanceIndexer() :this(Guid.NewGuid().ToString()){ }
        public InstanceIndexer(string id)
        {
            ID = id;
            Instances.Add(ID,this as T);
        }
        ~InstanceIndexer()
        {
            if (Instances.ContainsKey(ID))
                Instances.Remove(ID);
        }
    }
}
