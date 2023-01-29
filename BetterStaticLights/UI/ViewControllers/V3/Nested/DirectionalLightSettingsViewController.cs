using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3.Nested
{
    [ViewDefinition("BetterStaticLights.UI.BSML.NestedV3.directionals.bsml")]
    [HotReload(RelativePathToLayout = "../../../BSML/NestedV3/directionals.bsml")]
    public class DirectionalLightSettingsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly V3LightSettingsViewController lightSettingsViewController;

        [UIAction("back-button-was-pressed")]
        public void HandleBackButtonWasPressed()
        {
            lightSettingsViewController.HandleSubmenuButtonWasPressed(false);
        }
    }
}
