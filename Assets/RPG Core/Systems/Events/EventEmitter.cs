using RPG.Core.SerializableDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.Events
{
    //[Serializable]
    public class EventEmitter : ISerializationCallbackReceiver
    {
        private static SerializableDictionaryStringRPGEvent _events = new SerializableDictionaryStringRPGEvent();

        [SerializeField]
        private SerializableDictionaryStringRPGEvent _gameEvents;
        public static bool AllowDynamicEventCreation { get; set; } = false;

        public static event Action OnReset;

        private static event Action _globalUnsubEvent;
        public static void UnsubscribeFromAllGlobal() {
            Debug.LogWarning("A global reset of all events was triggered by calling UnsubscribeFromAllGlobal(). This function should only be called when exiting the game or when changing scenes");
            OnReset?.Invoke();
            _globalUnsubEvent?.Invoke();
        }
        private event Action _instanceUnsubEvent;
        //Unsubscribes from all the events subscribed to by this EventEmitter instance.
        public void UnsubscribeFromAll()
        {
            _instanceUnsubEvent?.Invoke();
        }

        //Parameterless events
        public Action Subscribe<TEnum>(TEnum eventEnum,Action<object> callback) where TEnum : struct, Enum
        {
            return Subscribe(eventEnum.ToString(),callback);
        }
        public Action Subscribe(string eventName,Action<object> callback)
        {
            var unsub = InternalSubscribe(eventName,callback);
            _instanceUnsubEvent += unsub;
            return unsub;
        }
        private static Action InternalSubscribe(string eventName,Action<object> callback)
        {
            EventHandler<RPGEventArgs> handler = (sender,args) => {
                callback(sender);
            };

            if (_debug) Debug.Log($"Subscribing {callback} to event {eventName}");
            if (!_events.ContainsKey(eventName))
            {
                if (AllowDynamicEventCreation) _events.Add(eventName,new RPGEvent<RPGEventArgs>());
                else throw new KeyNotFoundException($"A event with the name {eventName} does not exist in the game emitter and AllowDynamicEventCreation is set to false");
            }
            
            _events[eventName].AddListener(handler);

            Action unsub = () => { _events[eventName].RemoveListener(handler); };
            _globalUnsubEvent += unsub;
            return unsub;
        }
        public void Emit<TEnum>(TEnum eventEnum,object sender) where TEnum : struct, Enum
        {
            Emit(eventEnum.ToString(),sender);
        }
        public void Emit(string eventName,object sender)
        {
            InternalEmit(eventName,sender);
        }
        private static void InternalEmit(string eventName, object sender) {
            if (!_events.ContainsKey(eventName)) return;
            RPGEvent<RPGEventArgs> handler = _events[eventName];
            if (_debug) Debug.Log($"Emiting event {eventName} from {sender}");
            handler?.Invoke(sender,RPGEventArgs.Empty);
        }
        public void Unsubscribe<TEnum>(TEnum eventName,Action<object> callback) where TEnum : struct, Enum
        {
            Unsubscribe(eventName.ToString(),callback);
        }
        public void Unsubscribe(string eventName,Action<object> callback)
        {
            if (!_events.ContainsKey(eventName)) return;
            var handler = _events[eventName];
            handler.RemoveListener((sender,args) => 
            { 
                callback(sender); 
            });
        }
        //Events with single argument provided where the argumnet is a class decended from EventArgs
        public Action Subscribe<TEnum, T>(TEnum eventName,Action<object,T> callback) where TEnum : struct, Enum where T : RPGEventArgs
        {
            return Subscribe<T>(eventName.ToString(),callback);
        }
        public Action Subscribe<T>(string eventName,Action<object,T> callback) where T : RPGEventArgs
        {
            var unsub = InternalSubscribe(eventName,callback);
            _instanceUnsubEvent += unsub;
            return unsub;
        }
        private static Action InternalSubscribe<T>(string eventName,Action<object,T> callback) where T : RPGEventArgs
        {
            EventHandler<RPGEventArgs> handler = (sender,args) => {
                callback(sender,(T)args);
            };

            if (_debug) Debug.Log($"Subscribing {callback} to event {eventName}");
            if (!_events.ContainsKey(eventName)) {
                if (AllowDynamicEventCreation)
                    _events.Add(eventName,new RPGEvent<RPGEventArgs>());
                else throw new KeyNotFoundException($"A event with the name {eventName} does not exist in the game emitter and AllowDynamicEventCreation is set to false");
            }

            _events[eventName].AddListener(handler);

            Action unsub = () => { _events[eventName].RemoveListener(handler); };
            _globalUnsubEvent += unsub;
            return unsub;
        }
        public void Emit<TEnum, T>(TEnum eventName,object sender,T args) where TEnum : struct, Enum where T : RPGEventArgs
        {
            Emit<T>(eventName.ToString(),sender,args);
        }
        public void Emit<T>(string eventName,object sender,T args) where T : RPGEventArgs
        {
            InternalEmit(eventName,sender,args);
        }
        private static void InternalEmit<T>(string eventName,object sender, T args) where T : RPGEventArgs
        {
            if (!_events.ContainsKey(eventName)) return;
            RPGEvent<RPGEventArgs> handler = _events[eventName];
            if (_debug) Debug.Log($"Emiting event {eventName} from {sender} with arguments {args}");
            handler?.Invoke(sender,args);
        }
        public void UnSubscribe<TEnum, T>(TEnum eventName,Action<object,T> callback) where TEnum : struct, Enum where T : RPGEventArgs
        {
            Unsubscribe<T>(eventName.ToString(),callback);
        }
        public void Unsubscribe<T>(string eventName, Action<object,T> callback) where T : RPGEventArgs
        {
            if (!_events.ContainsKey(eventName)) return;
            var handler = _events[eventName];
            handler.RemoveListener((sender, args) =>
            {
                callback(sender,(T)args);
            });
        }
        public static bool ToggleDebug() {
            _debug = _debug == true ? false : true;
            return _debug;
        }
        private static bool _debug = false;
        public void OnBeforeSerialize()
        {
            _gameEvents = _events;
        }
        public void OnAfterDeserialize()
        {
            //_events = _gameEvents;
        }
    }
}