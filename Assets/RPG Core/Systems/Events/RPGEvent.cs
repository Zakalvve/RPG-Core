using System;
using UnityEngine;

namespace RPG.Core.Events
{
    [Serializable]
    public class RPGEvent<T> : ISerializationCallbackReceiver where T : RPGEventArgs
    {
        public event EventHandler<T> Event;
        [SerializeField]
        private int numberOfSubscribers;
        public void Invoke(object sender, T args)
        {
            Event?.Invoke(sender, args);
        }
        public void AddListener(EventHandler<T> listener)
        {
            Event += listener;
        }
        public void RemoveListener(EventHandler<T> listener)
        {
            Event -= listener;
        }

        public void OnAfterDeserialize() 
        {
            numberOfSubscribers = Event?.GetInvocationList().Length ?? 0;
        }

        public void OnBeforeSerialize()
        {
            numberOfSubscribers = Event?.GetInvocationList().Length ?? 0;
        }
    }
}