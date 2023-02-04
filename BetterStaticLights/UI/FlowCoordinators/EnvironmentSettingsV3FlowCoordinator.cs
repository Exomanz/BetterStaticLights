using BeatSaberMarkupLanguage;
using BetterStaticLights.UI.ViewControllers;
using HMUI;
using Zenject;

namespace BetterStaticLights.UI.FlowCoordinators
{
    internal class EnvironmentSettingsV3FlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly BSLParentFlowCoordinator mainModFlowCoordinator;
        [Inject] private readonly V3ActiveSceneSettingsViewController sceneViewController;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                this.SetTitle("Environment Settings - V3");
                this.showBackButton = true;
                base.ProvideInitialViewControllers(sceneViewController);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);
            transitionHelper?.RefreshPreviewer(false);
            mainModFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
        }
    }
}
