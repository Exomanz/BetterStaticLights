using BeatSaberMarkupLanguage.GameplaySetup;
using BetterStaticLights.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPA.Logging;
using IPA.Utilities;
using System.Reflection;
using IPAConfig = IPA.Config.Config;

namespace BetterStaticLights
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Plugin Instance { get; private set; }

        internal readonly PluginConfig Config;
        internal readonly Harmony harmony;
        internal const string _harmonyId = "com.beatsaber.exo.betterstaticlights";

        [Init]
        public Plugin(Logger logger, IPAConfig config)
        {
            Instance = this;

            Config = config.Generated<PluginConfig>();
            harmony = new Harmony(_harmonyId);
        }

        [OnEnable]
        public void Enable()
        {
            Config.lightSets.Add(Config.BackTop);
            Config.lightSets.Add(Config.RingLights);
            Config.lightSets.Add(Config.LeftLasers);
            Config.lightSets.Add(Config.RightLasers);
            Config.lightSets.Add(Config.BottomBackSide);

            GameplaySetup.instance.AddTab("Better Static Lights", "BetterStaticLights.Settings.settings.bsml", SettingsUI.instance);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void Disable()
        {
            Config.lightSets.Clear();

            GameplaySetup.instance.RemoveTab("Better Static Lights");
            harmony.UnpatchSelf();
        }
    }
}