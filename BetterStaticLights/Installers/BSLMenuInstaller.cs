using BetterStaticLights.UI;
using BetterStaticLights.UI.FlowCoordinators;
using BetterStaticLights.UI.ViewControllers;
using BetterStaticLights.UI.ViewControllers.V2;
using UnityEngine;
using Zenject;

namespace BetterStaticLights.Installers
{
    internal class BSLMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<BSLParentFlowCoordinator>().FromNewComponentOn(new GameObject("BSL - Parent FlowCoordinator")).AsSingle();
            Container.Bind<EnvironmentSettingsV2FlowCoordinator>().FromNewComponentOn(new GameObject("BSL - V2 Settings FlowCoordinator")).AsSingle();

            BindViewController<MainBSLViewController>();
            BindViewController<V2LightSettingsViewController>();
            BindViewController<V2InfoViewController>();

            Container.BindInterfacesAndSelfTo<MenuButtonManager>().AsSingle().NonLazy();
        }

        private void BindViewController<T>() where T : HMUI.ViewController
        {
            Container.Bind<T>().FromNewComponentAsViewController().AsSingle();
        }
    }
}
