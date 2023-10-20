using Assets.RPG_Core.Core;
using System;

namespace RPG.Core.Events
{
    //the base event args for all events, an ID value can be sent if required
    public class RPGEventArgs : IIDable
    {
        public static RPGEventArgs Empty => new RPGEventArgs();
        public bool HasId => !String.IsNullOrEmpty(ID);

        public string ID { get; }

        public RPGEventArgs()
        {
            ID = null;
        }
        public RPGEventArgs(string id)
        {
            ID = id;
        }
    }
}