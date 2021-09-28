using BeatSaberMarkupLanguage.Attributes;

namespace BetterStaticLights.Settings
{
    internal class SettingsUI : PersistentSingleton<SettingsUI>
    {
        private static Config Config => Plugin.Config;

        [UIValue("back-top-enabled")]
        protected bool BackTopEnabled
        {
            get => Config.BackTop.Enabled;
            set => Config.BackTop.Enabled = value;
        }

        [UIValue("back-top-color")]
        protected bool BackTopColor
        {
            get => Config.BackTop.UseSecondaryColor;
            set => Config.BackTop.UseSecondaryColor = value;
        }

        [UIValue("ring-lights-enabled")]
        protected bool RingLightsEnabled
        {
            get => Config.RingLights.Enabled;
            set => Config.RingLights.Enabled = value;
        }

        [UIValue("ring-lights-color")]
        protected bool RingLightsColor
        {
            get => Config.RingLights.UseSecondaryColor;
            set => Config.RingLights.UseSecondaryColor = value;
        }

        [UIValue("left-lasers-enabled")]
        protected bool LeftLaserEnabled
        {
            get => Config.LeftLasers.Enabled;
            set => Config.LeftLasers.Enabled = value;
        }

        [UIValue("left-lasers-color")]
        protected bool LeftLasersColor
        {
            get => Config.LeftLasers.UseSecondaryColor;
            set => Config.LeftLasers.UseSecondaryColor = value;
        }

        [UIValue("right-lasers-enabled")]
        protected bool RightLasersEnabled
        {
            get => Config.RightLasers.Enabled;
            set => Config.RightLasers.Enabled = value;
        }

        [UIValue("right-lasers-color")]
        protected bool RightLasersColor
        {
            get => Config.RightLasers.UseSecondaryColor;
            set => Config.RightLasers.UseSecondaryColor = value;
        }

        [UIValue("bottom-back-side-enabled")]
        protected bool BottomBackSideEnabled
        {
            get => Config.BottomBackSide.Enabled;
            set => Config.BottomBackSide.Enabled = value;
        }

        [UIValue("bottom-back-side-color")]
        protected bool BottomBackSideColor
        {
            get => Config.BottomBackSide.UseSecondaryColor;
            set => Config.BottomBackSide.UseSecondaryColor = value;
        }
    }
}