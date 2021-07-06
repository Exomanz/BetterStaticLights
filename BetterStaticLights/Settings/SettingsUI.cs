using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;

namespace BetterStaticLights.Settings
{
    internal class SettingsUI : PersistentSingleton<SettingsUI>
    {
        static Config Config => Plugin.XConfig;

        [UIValue("AllLightOptions")]
        public List<object> LightChoices = Config.LightChoices;

        [UIValue("ChoiceOne")]
        protected string ChoiceOne
        {
            get => Config.Choice1;
            set
            {
                Config.Choice1 = value;
                Plugin.GenerateIL();
            }
        }

        [UIValue("ChoiceTwo")]
        protected string ChoiceTwo
        {
            get => Config.Choice2;
            set
            {
                Config.Choice2 = value;
                Plugin.GenerateIL();
            }
        }
    }
}