using BetterStaticLights.UI;
using BetterStaticLights.UI.FlowCoordinators;
using BetterStaticLights.UI.ViewControllers;
using HMUI;
using UnityEngine;
using Zenject;

namespace BetterStaticLights.Installers
{
    internal class BSLMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MockSceneTransitionHelper>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuButtonManager>().AsSingle().NonLazy();

            Container.Bind<BSLParentFlowCoordinator>().FromNewComponentOn(new GameObject("BSL - Parent FlowCoordinator")).AsSingle();
            Container.Bind<EnvironmentSettingsV2FlowCoordinator>().FromNewComponentOn(new GameObject("BSL - V2 Settings FlowCoordinator")).AsSingle();
            Container.Bind<EnvironmentSettingsV3FlowCoordinator>().FromNewComponentOn(new GameObject("BSL - V3 Settings FlowCoordinator")).AsSingle();

            BindViewController<MainBSLViewController>();
            BindViewController<V2LightSettingsViewController>();
            BindViewController<V2InfoViewController>();
            BindViewController<V3ActiveSceneSettingsViewController>();
        }

        private void BindViewController<T>() where T : ViewController
        {
            Container.Bind<T>().FromNewComponentAsViewController().AsSingle();
        }
    }
}
