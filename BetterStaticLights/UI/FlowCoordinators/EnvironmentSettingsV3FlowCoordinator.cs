using BeatSaberMarkupLanguage;
using BetterStaticLights.UI.ViewControllers;
using BetterStaticLights.UI.ViewControllers.V3;
using BetterStaticLights.UI.ViewControllers.V3.Nested;
using HMUI;
using Zenject;

namespace BetterStaticLights.UI.FlowCoordinators
{
    internal class EnvironmentSettingsV3FlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly BSLParentFlowCoordinator mainModFlowCoordinator;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;

        // Main ViewControllers
        [Inject] private readonly V3LightSettingsViewController settingsViewController;
        [Inject] private readonly V3ActiveSceneSettingsViewController sceneViewController;

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
            mainModFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
        }

        public void ReplaceRightScreenViewController(ViewController viewController, ViewController.AnimationType animationType)
        {
            base.SetRightScreenViewController(viewController, animationType);
        }
    }
}
