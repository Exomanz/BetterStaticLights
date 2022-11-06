//using BetterStaticLights.Installers;
using BetterStaticLights.Helpers;
using BetterStaticLights.Installers;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System.Collections.Generic;
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
        internal readonly Harmony harmony = new("com.beatsaber.exo.betterstaticlights");
        internal IPALogger Logger;

        [Init]
        public Plugin(IPAConfig config, IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Config = config.Generated<PluginConfig>();
            Logger = logger;

            zenjector.UseLogger(logger);
            zenjector.Install(Location.App, (Container) =>
            {
                Container.Bind<PluginConfig>().FromInstance(this.Config).AsCached();
            });
            zenjector.Install<BSLMenuInstaller>(Location.Menu);
        }

        [OnEnable]
        public void Enable()
        {
#if DEBUG
            GAMEOBJECTNAMEGETTER DEBUG = new GameObject("NAMEGETTER").AddComponent<GAMEOBJECTNAMEGETTER>();
            Object.DontDestroyOnLoad(DEBUG);
#endif
            this.PopulateLightSetList();
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void Disable()
        {
            this.PopulateLightSetList(false);
            harmony.UnpatchSelf();
        }

        private void PopulateLightSetList(bool state = true)
        {
            List<LightSetV2> setList = Config.lightSets;

            if (!state) setList.Clear();
            else
            {
                setList.Add(Config.LS_BackTop);
                setList.Add(Config.LS_RingLights);
                setList.Add(Config.LS_LeftLasers);
                setList.Add(Config.LS_RightLasers);
                setList.Add(Config.LS_BottomBackSide);
            }
        }
    }
}