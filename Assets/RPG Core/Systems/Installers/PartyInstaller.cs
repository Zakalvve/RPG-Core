using InventorySystem;
using Zenject;
using RPG.Core.Character;
using System.Collections.Generic;

namespace RPG.Core.Installers
{
    public class PartyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IParty>().To<PartyController>().FromComponentOnRoot().AsSingle().NonLazy();
            Container.Bind<IPartyStash>().To<FilterableInventory>().FromFactory<Factories.PartyStashFactory>().AsSingle();
        }
    }
}