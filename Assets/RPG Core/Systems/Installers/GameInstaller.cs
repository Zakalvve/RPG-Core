using RPG.Core.Events;
using UnityEngine;
using Zenject;

namespace RPG.Core.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private EventEmitter GameEmitter;
        public override void InstallBindings()
        {
            EventEmitter.AllowDynamicEventCreation = true;
            Container.Bind<EventEmitter>().AsTransient().NonLazy();
        }
    }
}