using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v2settings.bsml")]
    [HotReload(RelativePathToLayout = "../BSML/v2settings.bsml")]
    public class V2LightSettingsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly PluginConfig config;

        #region Enabled
        [UIValue("back-enabled")]
        public bool BackTop_Enabled
        {
            get => config.BackTop.enabled;
            set => config.BackTop.enabled = value;
        }

        [UIValue("ring-enabled")]
        public bool RingLights_Enabled
        {
            get => config.RingLights.enabled;
            set => config.RingLights.enabled = value;
        }

        [UIValue("left-enabled")]
        public bool LeftLasers_Enabled
        {
            get => config.LeftLasers.enabled;
            set => config.LeftLasers.enabled = value;
        }

        [UIValue("right-enabled")]
        public bool RightLasers_Enabled
        {
            get => config.RightLasers.enabled;
            set => config.RightLasers.enabled = value;
        }

        [UIValue("bottom-enabled")]
        public bool BottomBackSide_Enabled
        {
            get => config.BottomBackSide.enabled;
            set => config.BottomBackSide.enabled = value;
        }
        #endregion

        #region Color Switching
        [UIValue("back-secondary")]
        public bool BackTop_Secondary
        {
            get => config.BackTop.useSecondaryColor;
            set => config.BackTop.useSecondaryColor = value;
        }

        [UIValue("ring-secondary")]
        public bool RingLights_Secondary
        {
            get => config.RingLights.useSecondaryColor;
            set => config.RingLights.useSecondaryColor = value;
        }

        [UIValue("left-secondary")]
        public bool LeftLasers_Secondary
        {
            get => config.LeftLasers.useSecondaryColor;
            set => config.LeftLasers.useSecondaryColor = value;
        }

        [UIValue("right-secondary")]
        public bool RightLasers_Secondary
        {
            get => config.RightLasers.useSecondaryColor;
            set => config.RightLasers.useSecondaryColor = value;
        }

        [UIValue("bottom-secondary")]
        public bool BottomBackSide_Secondary
        {
            get => config.BottomBackSide.useSecondaryColor;
            set => config.BottomBackSide.useSecondaryColor = value;
        }
        #endregion
    }
}