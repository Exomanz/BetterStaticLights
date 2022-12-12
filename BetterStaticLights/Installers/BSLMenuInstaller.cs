using BetterStaticLights.UI;
using BetterStaticLights.UI.FlowCoordinators;
using BetterStaticLights.UI.ViewControllers;
using BetterStaticLights.UI.ViewControllers.V2;
using BetterStaticLights.UI.ViewControllers.V3;
using HMUI;
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
            Container.Bind<EnvironmentSettingsV3FlowCoordinator>().FromNewComponentOn(new GameObject("BSL - V3 Settings FlowCoordinator")).AsSingle();
            Container.BindInterfacesAndSelfTo<MockSceneTransitionHelper>().AsSingle();

            // Settings Host
            BindViewController<MainBSLViewController>();
            
            // V2
            BindViewController<V2LightSettingsViewController>();
            BindViewController<V2InfoViewController>();

            // V3
            BindViewController<V3LightSettingsViewController>();
            BindViewController<V3ActiveSceneSettingsMenu>();

            Container.BindInterfacesAndSelfTo<MenuButtonManager>().AsSingle().NonLazy();
        }

        private void BindViewController<T>() where T : ViewController
        {
            Container.Bind<T>().FromNewComponentAsViewController().AsSingle();
        }
    }
}
