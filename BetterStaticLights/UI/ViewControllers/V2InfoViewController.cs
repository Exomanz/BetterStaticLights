using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace BetterStaticLights.UI.ViewControllers
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v2info.bsml")]
    [HotReload(RelativePathToLayout = "../BSML/v2info.bsml")]
    public class V2InfoViewController : BSMLAutomaticViewController
    {
    }
}
