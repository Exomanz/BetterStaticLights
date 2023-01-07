using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.UI.FlowCoordinators;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3.Nested
{
    [ViewDefinition("BetterStaticLights.UI.BSML.NestedV3.lightgroups.bsml")]
    [HotReload(RelativePathToLayout = "../../../BSML/NestedV3/lightgroups.bsml")]
    public class LightGroupSettingsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly V3LightSettingsViewController lightSettingsViewController;

        [UIAction("back-button-was-pressed")]
        public void HandleBackButtonWasPressed()
        {
            lightSettingsViewController.HandleSubmenuButtonWasPressed(false);
        }
    }
}
