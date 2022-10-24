//using BetterStaticLights.Installers;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using UnityEngine;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace BetterStaticLights
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        public static Plugin Instance { get; private set; }
        internal readonly PluginConfig Config;
        internal const string _harmonyID= "com.beatsaber.exomanz.betterstaticlights";
        internal readonly Harmony harmony = new(_harmonyID);
        internal IPALogger Logger;

        [Init]
        public Plugin(IPAConfig config, IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Config = config.Generated<PluginConfig>();
            Logger = logger;

            zenjector.UseLogger(logger);
        }

        [OnEnable]
        public void Enable()
        {
#if DEBUG
            GAMEOBJECTNAMEGETTER DEBUG = new GameObject("NAMEGETTER").AddComponent<GAMEOBJECTNAMEGETTER>();
            Object.DontDestroyOnLoad(DEBUG);
#endif
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void Disable()
        {
            harmony.UnpatchSelf();
        }
    }
}