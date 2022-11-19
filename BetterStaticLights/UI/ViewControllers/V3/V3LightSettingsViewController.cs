using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace BetterStaticLights.UI.ViewControllers.V3
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3settings.bsml")]
    [HotReload(RelativePathToLayout = "../../BSML/v3settings.bsml")]
    internal class V3LightSettingsViewController : BSMLAutomaticViewController
    {
    }
}
