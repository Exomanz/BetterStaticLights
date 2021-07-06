using BeatSaberMarkupLanguage.GameplaySetup;
using BetterStaticLights.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
using System.Reflection;
using System.Reflection.Emit;

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

            GenerateIL();
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

        public static void GenerateIL()
        {
            if (XConfig.Choice1 == XConfig.Choice2)
                Logger.Warn("Both choices are identical--only one light effect will show. Was this intentional?");

            switch (XConfig.Choice1)
            {
                case "BackTop":
                    Config.opCode1 = OpCodes.Ldc_I4_0;
                    break;
                case "RingLights":
                    Config.opCode1 = OpCodes.Ldc_I4_1;
                    break;
                case "LeftLasers":
                    Config.opCode1 = OpCodes.Ldc_I4_2;
                    break;
                case "RightLasers":
                    Config.opCode1 = OpCodes.Ldc_I4_3;
                    break;
                case "BottomBackSide":
                    Config.opCode1 = OpCodes.Ldc_I4_4;
                    break;
                case "Off":
                    Config.opCode1 = OpCodes.Ldc_I4_M1;
                    break;
            }

            switch (XConfig.Choice2)
            {
                case "BackTop":
                    Config.opCode2 = OpCodes.Ldc_I4_0;
                    break;
                case "RingLights":
                    Config.opCode2 = OpCodes.Ldc_I4_1;
                    break;
                case "LeftLasers":
                    Config.opCode2 = OpCodes.Ldc_I4_2;
                    break;
                case "RightLasers":
                    Config.opCode2 = OpCodes.Ldc_I4_3;
                    break;
                case "BottomBackSide":
                    Config.opCode2 = OpCodes.Ldc_I4_4;
                    break;
                case "Off":
                    Config.opCode2 = OpCodes.Ldc_I4_M1;
                    break;
            }

            if (HarmonyID != null) HarmonyID.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
