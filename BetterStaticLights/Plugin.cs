using BeatSaberMarkupLanguage.GameplaySetup;
using BetterStaticLights.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPAConfig = IPA.Config.Config;

namespace BetterStaticLights
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Config Config { get; private set; }
        internal static Harmony _Harmony { get; private set; }
        internal const string Harmony_ID = "bs.Exomanz.bsl";

        [Init]
        public Plugin(IPAConfig config)
        {
            Config = config.Generated<Config>();
        }

        [OnEnable]
        public void Enable()
        {
            _Harmony = Harmony.CreateAndPatchAll(typeof(HarmonyPatches.NoEffectsTransformPatch), Harmony_ID);
            GameplaySetup.instance.AddTab("Better Static Lights", "BetterStaticLights.Settings.settingsPage.bsml", SettingsUI.instance);
        }

        [OnDisable]
        public void Disable()
        {
            GameplaySetup.instance.RemoveTab("Better Static Lights");
            _Harmony?.UnpatchSelf();
            _Harmony = null;
        }
    }
}