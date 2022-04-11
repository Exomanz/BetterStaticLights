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

        internal bool incompatibleVersion = false;
        internal readonly PluginConfig Config;
        internal readonly Harmony harmony;
        internal const string _harmonyId = "bs.Exo.BetterStaticLights";

        [Init]
        public Plugin(Logger logger, IPAConfig config)
        {
            if (UnityGame.GameVersion < new AlmostVersion("1.21.0", AlmostVersion.StoredAs.SemVer))
            {
                logger.Error("BetterStaticLights is running on an old version of Beat Saber!");
                logger.Error("Core functionality changed with 1.21.0, so this plugin will not work, and therefore will not initialize.");
                logger.Error("Please download an older version of the mod to use it on a version prior to 1.21.0.");

                incompatibleVersion = true;
            }

            Instance = this;

            Config = config.Generated<PluginConfig>();
            harmony = new Harmony(_harmonyId);
        }

        [OnEnable]
        public void Enable()
        {
            if (incompatibleVersion) return;

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
            if (incompatibleVersion) return;

            Config.lightSets.Clear();

            GameplaySetup.instance.RemoveTab("Better Static Lights");
            harmony.UnpatchSelf();
        }
    }
}