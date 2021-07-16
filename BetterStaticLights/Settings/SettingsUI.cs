using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterStaticLights.Settings
{
    internal class SettingsUI : PersistentSingleton<SettingsUI>
    {
        private static Config Config => Plugin.XConfig;

        #region Random Stuff
        public static List<object> LightSets => Enum.GetNames(typeof(Config.LightSets)).ToList<object>();
#pragma warning disable IDE0051
        private string ChoiceOne { get; set; } = LightSets[Config.LightSetOne].ToString();
        private string ChoiceTwo { get; set; } = LightSets[Config.LightSetTwo].ToString();
#pragma warning restore IDE0051
        #endregion

        [UIAction("ApplySetOne")]
        public void ApplySetOne(string set)
        {
            Config.LightSetOne = LightSets.IndexOf(set);
            Generate();
        }

        [UIAction("ApplySetTwo")]
        public void ApplySetTwo(string set)
        {
            Config.LightSetTwo = LightSets.IndexOf(set);
            Generate();
        }

        [UIValue("ColorForSetOne")]
        protected bool ColorForSetOne
        {
            get => Config.UseSecondarySaberColor_SetOne;
            set
            {
                Config.UseSecondarySaberColor_SetOne = value;
                Generate();
            }
        }

        [UIValue("ColorForSetTwo")]
        protected bool ColorForSetTwo
        {
            get => Config.UseSecondarySaberColor_SetTwo;
            set
            {
                Config.UseSecondarySaberColor_SetTwo = value;
                Generate();
            }
        }

        private void Generate() => ILGenerator.Generate();
    }
}