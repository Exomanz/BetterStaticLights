using BeatSaberMarkupLanguage;
using BetterStaticLights.UI.ViewControllers.V3;
using HMUI;
using Zenject;

namespace BetterStaticLights.UI.FlowCoordinators
{
    internal class EnvironmentSettingsV3FlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly BSLParentFlowCoordinator parentFlowCoordinator;
        [Inject] private readonly V3LightSettingsViewController settingsViewController;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                this.SetTitle("Environment Settings - V3");
                this.showBackButton = true;
                base.ProvideInitialViewControllers(settingsViewController);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);
            parentFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
        }
    }
}
