using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using SiraUtil.Logging;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V2
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v2settings.bsml")]
    [HotReload(RelativePathToLayout = "../../BSML/v2settings.bsml")]
    public class V2LightSettingsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly SiraLog logger;
        [Inject] private readonly PluginConfig config;

        #region Enable
        [UIValue("back-enabled")]
        public bool BackTop_Enabled
        {
            get => config.LS_BackTop.enabled;
            set => config.LS_BackTop.enabled = value;
        }

        [UIValue("ring-enabled")]
        public bool RingLights_Enabled
        {
            get => config.LS_RingLights.enabled;
            set => config.LS_RingLights.enabled = value;
        }

        [UIValue("left-enabled")]
        public bool LeftLasers_Enabled
        {
            get => config.LS_LeftLasers.enabled;
            set => config.LS_LeftLasers.enabled = value;
        }

        [UIValue("right-enabled")]
        public bool RightLasers_Enabled
        {
            get => config.LS_RightLasers.enabled;
            set => config.LS_RightLasers.enabled = value;
        }

        [UIValue("bottom-enabled")]
        public bool BottomBackSide_Enabled
        {
            get => config.LS_BottomBackSide.enabled;
            set => config.LS_BottomBackSide.enabled = value;
        }
        #endregion

        #region Color Switching
        [UIValue("back-secondary")]
        public bool BackTop_Secondary
        {
            get => config.LS_BackTop.useSecondaryColor;
            set => config.LS_BackTop.useSecondaryColor = value;
        }

        [UIValue("ring-secondary")]
        public bool RingLights_Secondary
        {
            get => config.LS_RingLights.useSecondaryColor;
            set => config.LS_RingLights.useSecondaryColor = value;
        }

        [UIValue("left-secondary")]
        public bool LeftLasers_Secondary
        {
            get => config.LS_LeftLasers.useSecondaryColor;
            set => config.LS_LeftLasers.useSecondaryColor = value;
        }

        [UIValue("right-secondary")]
        public bool RightLasers_Secondary
        {
            get => config.LS_RightLasers.useSecondaryColor;
            set => config.LS_RightLasers.useSecondaryColor = value;
        }

        [UIValue("bottom-secondary")]
        public bool BottomBackSide_Secondary
        {
            get => config.LS_BottomBackSide.useSecondaryColor;
            set => config.LS_BottomBackSide.useSecondaryColor = value;
        }
        #endregion
    }
}