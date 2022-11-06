using BeatSaberMarkupLanguage;
using BetterStaticLights.UI.ViewControllers;
using HMUI;
using Zenject;

namespace BetterStaticLights.UI.FlowCoordinators
{
    public class BSLParentFlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly MainBSLViewController mainBSLViewController;
        [Inject] private readonly MainFlowCoordinator mainFlowCoordinator;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                this.showBackButton = true;
                this.SetTitle("Better Static Lights");
                base.ProvideInitialViewControllers(mainBSLViewController);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);
            mainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
