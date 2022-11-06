using BeatSaberMarkupLanguage;
using BetterStaticLights.UI.ViewControllers.V2;
using HMUI;
using Zenject;

namespace BetterStaticLights.UI.FlowCoordinators
{
    internal class EnvironmentSettingsV2FlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly BSLParentFlowCoordinator parentFlowCoordinator;
        [Inject] private readonly V2LightSettingsViewController settingsView;
        [Inject] private readonly V2InfoViewController v2InfoView;
        [Inject] private readonly PluginConfig config;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                this.SetTitle("Environment Settings - V2");
                this.showBackButton = true;
                base.ProvideInitialViewControllers(settingsView, v2InfoView);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);
            parentFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
        }
    }
}
