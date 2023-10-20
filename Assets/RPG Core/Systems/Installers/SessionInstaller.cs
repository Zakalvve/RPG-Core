using InventorySystem;
using RPG.Core.Character;
using System.Collections.Generic;
using Zenject;

namespace RPG.Core.Installers
{
    public class SessionInstaller : MonoInstaller
    {
        public GameObjectContext partyPrefab;
        public override void InstallBindings()
        {
            Container.Bind<IEquipmentConfig>().FromFactory<Factories.EquipConfigFactory>().AsSingle();
            Container.Bind<List<PartyCharacterController>>().FromFactory<Factories.PartyMemberFactory>().AsSingle().NonLazy();
            Container.Bind<IParty>().FromSubContainerResolve().ByNewContextPrefab(partyPrefab).AsSingle().NonLazy();
            Container.Bind<PartyController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ISystemMessenger>().To<SystemMessenger>().FromComponentInHierarchy().AsSingle();
        }
    }
}