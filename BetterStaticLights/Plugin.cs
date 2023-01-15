using BetterStaticLights.Configuration;
using BetterStaticLights.Installers;
using BetterStaticLights.Patches;
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
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Plugin Instance { get; private set; }
        internal bool DEBUG { get; private set; } = false;

        internal readonly PluginConfig Config;
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
            zenjector.Install<BSLAppInstaller>(Location.App, this.Config);
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
        }

        [OnDisable]
        public void Disable()
        {
            this.PopulateV2LightSetList(false);
        }

        private void PopulateV2LightSetList(bool state = true)
        {
            if (!state) Config.lightSets.Clear();
            else
            {
                Config.lightSets.Add(Config.BackTop);
                Config.lightSets.Add(Config.RingLights);
                Config.lightSets.Add(Config.LeftLasers);
                Config.lightSets.Add(Config.RightLasers);
                Config.lightSets.Add(Config.BottomBackSide);
            }
        }
    }
}