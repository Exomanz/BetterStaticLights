using BeatSaberMarkupLanguage.GameplaySetup;
using BetterStaticLights.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
using System.Reflection;

namespace BetterStaticLights
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static IPALogger Logger { get; private set; }
        internal static Harmony HarmonyID { get; private set; }
        internal static Config XConfig { get; private set; }

        [Init]
        public Plugin(IPAConfig iConfig, IPALogger iLogger)
        {
            Config config = iConfig.Generated<Config>();
            XConfig = config;
            Logger = iLogger;

            ILGenerator.Generate();
        }

        [OnEnable]
        public void Enable()
        {
            if (HarmonyID is null) HarmonyID = new Harmony("bs.Exomanz.BetterStaticLights");
            HarmonyID.PatchAll(Assembly.GetExecutingAssembly());
            GameplaySetup.instance.AddTab("Better Lights", "BetterStaticLights.Settings.settingsPage.bsml", SettingsUI.instance);
        }

        [OnDisable]
        public void Disable()
        {
            HarmonyID.UnpatchAll(HarmonyID.Id);
            HarmonyID = null;
            GameplaySetup.instance.RemoveTab("Better Lights");
        }
    }
}
