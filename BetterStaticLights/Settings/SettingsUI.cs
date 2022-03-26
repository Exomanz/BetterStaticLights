using BeatSaberMarkupLanguage.Attributes;
using TMPro;

namespace BetterStaticLights.Settings
{
    internal class SettingsUI : PersistentSingleton<SettingsUI>
    {
        private PluginConfig Config => Plugin.Instance.Config;

        [UIValue("bt-enabled")]
        public bool BTEnabled
        {
            get => Config.BackTop.Enabled;
            set => Config.BackTop.Enabled = value;
        }

        [UIValue("bt-secondary")]
        public bool BTSecondary
        {
            get => Config.BackTop.UseSecondaryColor;
            set => Config.BackTop.UseSecondaryColor = value;
        }

        [UIValue("rl-enabled")]
        public bool RLEnabled
        {
            get => Config.RingLights.Enabled;
            set => Config.RingLights.Enabled = value;
        }

        [UIValue("rl-secondary")]
        public bool RLSecondary
        {
            get => Config.RingLights.UseSecondaryColor;
            set => Config.RingLights.UseSecondaryColor = value;
        }

        [UIValue("ll-enabled")]
        public bool LLEnabled
        {
            get => Config.LeftLasers.Enabled;
            set => Config.LeftLasers.Enabled = value;
        }

        [UIValue("ll-secondary")]
        public bool LLSecondary
        {
            get => Config.LeftLasers.UseSecondaryColor;
            set => Config.LeftLasers.UseSecondaryColor = value;
        }

        [UIValue("ril-enabled")]
        public bool RILEnabled
        {
            get => Config.RightLasers.Enabled;
            set => Config.RightLasers.Enabled = value;
        }

        [UIValue("ril-secondary")]
        public bool RILSecondary
        {
            get => Config.RightLasers.UseSecondaryColor;
            set => Config.RightLasers.UseSecondaryColor = value;
        }

        [UIValue("bbs-enabled")]
        public bool BBSEnabled
        {
            get => Config.BottomBackSide.Enabled;
            set => Config.BottomBackSide.Enabled = value;
        }

        [UIValue("bbs-secondary")]
        public bool BBSSecondary
        {
            get => Config.BottomBackSide.UseSecondaryColor;
            set => Config.BottomBackSide.UseSecondaryColor = value;
        }
    }
}