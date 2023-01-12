using BetterStaticLights.Patches;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BetterStaticLights.Installers
{
    public class BSLGameInstaller : Installer
    {
        [Inject] private readonly SiraLog logger;

        public override void InstallBindings()
        {
        }
    }
}
