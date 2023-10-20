using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace RPG.Core.Installers
{
    public class PlayerInstaller : MonoInstaller<PlayerInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromComponentInChildren().AsSingle();
            Container.Bind<PlayerInput>().FromComponentOnRoot().AsSingle();
        }
    }
}
