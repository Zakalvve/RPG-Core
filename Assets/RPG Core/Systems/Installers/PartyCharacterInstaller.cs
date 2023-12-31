using InventorySystem;
using RPG.Core.Character;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using RPG.Core.Character.Attributes;

namespace RPG.Core.Installers
{
    public class PartyCharacterInstaller : MonoInstaller<PartyCharacterInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PartyCharacter>().FromComponentOnRoot().AsCached();
            Container.Bind<Inventory>().FromFactory<Factories.InventoryFactory>().AsSingle();
            Container.Bind<ActionBar>().FromFactory<Factories.ActionBarFactory>().AsSingle();
            Container.Bind<EquipmentInventory>().FromFactory<Factories.EquipmentInventoryFactory>().AsSingle();
            Container.Bind<MeshRenderer>().FromComponentsOnRoot().AsSingle();
            Container.Bind<NavMeshAgent>().FromComponentOnRoot().AsSingle();
            Container.Bind<PawnMovementController>().FromComponentOnRoot().AsSingle();
            Container.Bind<PartyCharacterController>().FromComponentOnRoot().AsSingle();
            Container.Bind<AttributeSet>().FromFactory<Factories.StatBlockFactory>().AsSingle();
        }
    }
}