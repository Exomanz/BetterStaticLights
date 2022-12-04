//using BetterStaticLights.Installers;
using BetterStaticLights.Configuration;
using BetterStaticLights.Installers;
using BetterStaticLights.Patches;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Logging;
using SiraUtil.Zenject;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace BetterStaticLights
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        public static Plugin Instance { get; private set; }

        internal readonly PluginConfig Config;
        internal readonly Harmony harmony = new Harmony("com.beatsaber.exo.betterstaticlights");
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
            zenjector.Install<EnvironmentSceneSetup>((Container) =>
            {
                var environmentInfo = Container.TryResolve<EnvironmentSceneSetupData>();
                if (environmentInfo != null)
                {
                    EnvironmentInfoSO info = environmentInfo.environmentInfo;
#if DEBUG
                    foreach (Scene scene in SceneManager.GetAllScenes())
                    {
                        logger.Info(scene.name);
                    }
#endif
                    logger.Info(info.sceneInfo.sceneName);
                    Container.Bind<V3EnvironmentLightOverrides>().FromNewComponentOn(new GameObject("LightOverrides")).AsSingle().WithArguments(info).NonLazy();
                }
            });
        }

        [OnEnable]
        public void Enable()
        {
#if DEBUG
            GAMEOBJECTNAMEGETTER DEBUG = new GameObject("NAMEGETTER").AddComponent<GAMEOBJECTNAMEGETTER>();
            Object.DontDestroyOnLoad(DEBUG);
#endif
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