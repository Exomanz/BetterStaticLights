using BeatSaberMarkupLanguage.Attributes;

namespace BetterStaticLights.Settings
{
    internal class SettingsUI : PersistentSingleton<SettingsUI>
    {
        private static Config Config => Plugin.XConfig;

        [UIValue("BackTop")] protected bool BackTop
        {
            get => Config.BackTop;
            set => Config.BackTop = value;
        }

        [UIValue("BTSecondary")] protected bool BTSecondary
        {
            get => Config.BTSecondaryColor;
            set => Config.BTSecondaryColor = value;
        }

        [UIValue("RingLights")] protected bool RingLights
        {
            get => Config.RingLights;
            set => Config.RingLights = value;
        }

        [UIValue("RLSecondary")] protected bool RLSecondary
        {
            get => Config.RLSecondaryColor;
            set => Config.RLSecondaryColor = value;
        }

        [UIValue("LeftLasers")] protected bool LeftLasers
        {
            get => Config.LeftLasers;
            set => Config.LeftLasers = value;
        }

        [UIValue("LLSecondary")] protected bool LLSecondary
        {
            get => Config.LLSecondaryColor;
            set => Config.LLSecondaryColor = value;
        }

        [UIValue("RightLasers")] protected bool RightLasers
        {
            get => Config.RightLasers;
            set => Config.RightLasers = value;
        }

        [UIValue("RLSSecondary")] protected bool RLSSecondary
        {
            get => Config.RLSSecondaryColor;
            set => Config.RLSSecondaryColor = value;
        }

        [UIValue("BottomBackSide")] protected bool BottomBackSide
        {
            get => Config.BottomBackSide;
            set => Config.BottomBackSide = value;
        }

        [UIValue("BBSSecondary")] protected bool BBSSecondary
        {
            get => Config.BBSSecondaryColor;
            set => Config.BBSSecondaryColor = value;
        }
    }
}