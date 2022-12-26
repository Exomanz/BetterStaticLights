using BetterStaticLights.Configuration;
using BetterStaticLights.Installers;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace BetterStaticLights
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        public static Plugin Instance { get; private set; }
        internal bool DEBUG { get; private set; } = false;

        internal readonly PluginConfig Config;
        internal readonly Harmony harmony = new Harmony("com.beatsaber.exo.betterstaticlights");
        internal IPALogger Logger;

        [Init]
        public Plugin(IPAConfig config, IPALogger logger, Zenjector zenjector)
        {
            if (Environment.GetCommandLineArgs().Any(arg => arg.Equals("--bsl-debug")))
                this.DEBUG = true;

            Instance = this;
            Config = config.Generated<PluginConfig>();
            Logger = logger;

            zenjector.UseLogger(logger);
            zenjector.Install(Location.App, (Container) =>
            {
                Container.Bind<PluginConfig>().FromInstance(this.Config).AsCached();
            });
            zenjector.Install<BSLMenuInstaller>(Location.Menu);
            zenjector.Install<BSLGameInstaller>(Location.StandardPlayer);
        }

        [OnEnable]
        public void Enable()
        {
            if (this.DEBUG)
            {
                GAMEOBJECTNAMEGETTER NAMEGETTER = new GameObject("BSLNAMEGETTER").AddComponent<GAMEOBJECTNAMEGETTER>();
                UnityEngine.Object.DontDestroyOnLoad(NAMEGETTER);
            }

            this.PopulateV2LightSetList();
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void Disable()
        {
            this.PopulateV2LightSetList(false);
            harmony.UnpatchSelf();
        }

        private void PopulateV2LightSetList(bool state = true)
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