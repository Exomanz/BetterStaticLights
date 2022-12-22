using BeatSaberMarkupLanguage;
using BetterStaticLights.UI.ViewControllers;
using BetterStaticLights.UI.ViewControllers.V3;
using HMUI;
using Zenject;

namespace BetterStaticLights.UI.FlowCoordinators
{
    internal class EnvironmentSettingsV3FlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly PluginConfig config;
        [Inject] private readonly MainBSLViewController mainBSLViewController;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly BSLParentFlowCoordinator parentFlowCoordinator;
        [Inject] private readonly V3LightSettingsViewController settingsViewController;
        [Inject] private readonly V3ActiveSceneSettingsMenu sceneViewController;

        public bool isInSettingsView = false;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                this.SetTitle("Environment Settings - V3");
                this.showBackButton = true;
                base.ProvideInitialViewControllers(sceneViewController, null, settingsViewController);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);
            base.StartCoroutine(transitionHelper.SetOrChangeEnvironmentPreview(false));
            parentFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
        }
    }
}
