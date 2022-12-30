using BeatSaberMarkupLanguage;
using BetterStaticLights.UI.FlowCoordinators;
using BetterStaticLights.UI.ViewControllers;
using BetterStaticLights.UI.ViewControllers.V3;
using IPA.Utilities.Async;
using SiraUtil.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BetterStaticLights.UI
{
    internal class MockSceneTransitionHelper : IInitializable, IDisposable
    {
        #region STATIC
        public static List<object> v3Environments { get; } = new List<object>()
        {
            "Weave",
            "Fall Out Boy",
            "EDM",
            "The Second",
            "Lizzo",
            "The Weeknd",
            "Rock Mixtape"
        };

        internal static Dictionary<string, string> sceneToNormalizedNames = new Dictionary<string, string>()
        {
            { "WeaveEnvironment", "Weave" },
            { "PyroEnvironment", "Fall Out Boy" },
            { "EDMEnvironment", "EDM" },
            { "TheSecondEnvironment", "The Second" },
            { "LizzoEnvironment", "Lizzo"},
            { "TheWeekndEnvironment", "The Weeknd" },
            { "RockMixtapeEnvironment", "Rock Mixtape" }
        };

        internal static Dictionary<string, string> normalizedToSceneNames = new Dictionary<string, string>()
        {
            { "Weave", "WeaveEnvironment" },
            { "Fall Out Boy", "PyroEnvironment" },
            { "EDM", "EDMEnvironment" },
            { "The Second", "TheSecondEnvironment" },
            { "Lizzo", "LizzoEnvironment" },
            { "The Weeknd", "TheWeekndEnvironment" },
            { "Rock Mixtape", "RockMixtapeEnvironment" }
        };

        public static string GetNormalizedSceneName(string sceneName)
        {
            sceneToNormalizedNames.TryGetValue(sceneName, out string value);
            return value;
        }

        public static string GetSerializableSceneName(string sceneName)
        {
            normalizedToSceneNames.TryGetValue(sceneName, out string value);
            return value;
        }
        #endregion

        [Inject] private readonly SiraLog logger;
        [Inject] private readonly MainBSLViewController mainViewController;
        [Inject] private readonly EnvironmentSettingsV3FlowCoordinator v3FlowCoordinator;
        [Inject] private readonly V3ActiveSceneSettingsMenu v3SceneSettings;
        [Inject] private readonly PluginConfig config;
        [Inject] private readonly StandardLevelDetailViewController levelView;
        [Inject] private readonly PlayerDataModel playerData;

        public string previouslyLoadedEnvironment = null;

        private List<GameObject> mockSceneObjects = new List<GameObject>();
        private bool hasCopiedEnvironmentElements = false;

        private Scene mockScene;
        private LightWithIdManager lightManager;
        private List<LightGroup> environmentLightGroups = new List<LightGroup>(501);

        public void Initialize()
        {
            // Cleanup cached objects when selecting the "Play" button to start a level
            levelView.didPressActionButtonEvent += this.Cleanup;
        }

        private void Cleanup(StandardLevelDetailViewController sldvc)
        {
            SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(false, destroyCachedEnvironmentObjects: true));
        }

        public IEnumerator SetOrChangeEnvironmentPreview(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool destroyCachedEnvironmentObjects = false)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
            {
                logger.Logger.Error($"Illegal argument given for string argument 'environmentName'.\nReceived: {environmentName ?? "null"}; Loading 'WeaveEnvironment'");
                config.environmentPreview = "WeaveEnvironment";
            }

            if (isEnteringPreviewState)
            {
                ColorScheme currentColorScheme = playerData.playerData.colorSchemesSettings.GetSelectedColorScheme();

                if (!hasCopiedEnvironmentElements)
                {
                    GameObject listParent = new GameObject("== BSL MOCK OBJECTS ==");
                    mockSceneObjects.Add(listParent);

                    GameObject env = null!;
                    GameObject core = null!;

                    SceneManager.LoadSceneAsync(environmentName, LoadSceneMode.Additive);
                    SceneManager.LoadSceneAsync("GameCore", LoadSceneMode.Additive);
                    yield return new WaitForSecondsRealtime(1f);

                    mockScene = SceneManager.GetSceneByName(environmentName);

                    env = Resources.FindObjectsOfTypeAll<EnvironmentSceneSetup>()[0]?.transform.parent.gameObject;
                    core = Resources.FindObjectsOfTypeAll<GameCoreSceneSetup>()[0]?.transform.parent.gameObject;

                    env.transform.SetParent(listParent.transform);
                    env.name = "BSLMock - Environment";

                    GameObject skybox = GameObject.Instantiate(core.transform.Find("BloomSkyboxQuad").gameObject, listParent.transform, true);
                    skybox.name = "BSLMock - BloomSkyboxQuad";
                    mockSceneObjects.Add(skybox);

                    // If ANY GameObject.Destroy() method fails, it throws a silent exception and the task return early, so I have to be VERY CAREFUL
                    yield return UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                    {
                        // Remove Saber Sparkle Managers
                        GameObject.Destroy(env.GetComponentInChildren<SaberBurnMarkArea>().gameObject);
                        GameObject.Destroy(env.GetComponentInChildren<SaberBurnMarkSparkles>().gameObject);

                        // Remove HUD
                        GameObject.Destroy(env.GetComponentInChildren<CoreGameHUDController>().gameObject);

                        // Remove exception-throwing LightSwitch managers
                        // The LightTranslationGroupEffectManager doesn't always exist, so that one has to get wrapped in a try block
                        env.GetComponentsInChildren<LightSwitchEventEffect>().ToList().ForEach(effect => GameObject.Destroy(effect.gameObject));
                        GameObject.Destroy(env.GetComponentInChildren<LightColorGroupEffectManager>().gameObject);
                        GameObject.Destroy(env.GetComponentInChildren<LightRotationGroupEffectManager>().gameObject);

                        logger.Logger.Info(environmentName);

                        switch (environmentName)
                        {
                            // LOL
                            case "WeaveEnvironment":
                                break;

                            // This environment is touchy...
                            case "PyroEnvironment":
                                env.GetComponentsInChildren<FireEffect>().ToList().ForEach(effect => GameObject.Destroy(effect));
                                GameObject.Destroy(env.GetComponentInChildren<SongTimeSyncedVideoPlayer>().gameObject);
                                GameObject.Destroy(env.GetComponentInChildren<EnvironmentStartEndSongAudioEffect>());
                                GameObject.Destroy(env.GetComponentInChildren<SpectrogramRow>());
                                GameObject.Destroy(env.GetComponentInChildren<SongTimeToShaderWriter>());
                                break;

                            // Simple enough
                            case "EDMEnvironment":
                                GameObject.Destroy(env.GetComponentInChildren<Spectrogram>());
                                break;

                            // Cool
                            case "TheSecondEnvironment":
                                env.GetComponentsInChildren<LightRotationEventEffect>().ToList().ForEach(effect => GameObject.Destroy(effect));
                                GameObject.Destroy(env.GetComponentInChildren<Spectrogram>());
                                GameObject.Destroy(env.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                GameObject.Destroy(env.GetComponentInChildren<SmoothStepPositionGroupEventEffect>());
                                break;

                            case "LizzoEnvironment":
                                GameObject.Destroy(env.GetComponentInChildren<ParticleSystemEmitEventEffect>().gameObject);
                                GameObject.Destroy(env.GetComponentInChildren<WhiteColorOrAlphaGroupEffectManager>().gameObject);
                                GameObject.Destroy(env.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                break;

                            // Pogchamp
                            case "TheWeekndEnvironment":
                                GameObject.Destroy(env.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                GameObject.Destroy(env.GetComponentInChildren<LightTranslationGroupEffectManager>().gameObject);
                                break;

                            case "RockMixtapeEnvironment":
                                GameObject.Destroy(env.GetComponentInChildren<LightTranslationGroupEffectManager>().gameObject);
                                GameObject.Destroy(env.GetComponentInChildren<SongTimeToShaderWriter>());
                                GameObject.Destroy(env.GetComponentInChildren<Spectrogram>());
                                GameObject.Destroy(env.GetComponentInChildren<EnvironmentStartEndSongAudioEffect>());
                                break;
                        }
                    });

                    mockSceneObjects.Add(env);
                    previouslyLoadedEnvironment = environmentName;
                    hasCopiedEnvironmentElements = true;

                    SceneManager.UnloadSceneAsync("GameCore", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    environmentLightGroups.Clear();
                    environmentLightGroups = env.GetComponentsInChildren<LightGroup>().ToList();
                    lightManager = env.GetComponentInChildren<LightWithIdManager>();
                    this.RegisterEnvironmentLights();
                    for (int i = 0; i < environmentLightGroups.Count; i++)
                    {
                        this.SetColorForGroup(environmentLightGroups[i], currentColorScheme.environmentColor1);
                    }
                }

                else if (!string.Equals(config.environmentPreview, previouslyLoadedEnvironment))
                {
                    mockSceneObjects.ForEach(obj => GameObject.Destroy(obj));
                    mockSceneObjects.Clear();
                    SceneManager.UnloadSceneAsync(mockScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    hasCopiedEnvironmentElements = false;
                    previouslyLoadedEnvironment = config.environmentPreview;

                    yield return SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(true, environmentName));
                }
                else
                {
                    // Refresh ColorScheme when the previewer is launched
                    for (int i = 0; i < environmentLightGroups.Count; i++)
                    {
                        this.SetColorForGroup(environmentLightGroups[i], currentColorScheme.environmentColor0);
                    }
                }
            }

            else if (destroyCachedEnvironmentObjects)
            {
                mockSceneObjects.ForEach(obj => GameObject.Destroy(obj));
                mockSceneObjects.Clear();
                hasCopiedEnvironmentElements = false;

                // Scenes are structs and cant be null so we have to check a property of it instead lol
                if (mockScene.name != null)
                    SceneManager.UnloadSceneAsync(mockScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                yield break;
            }

            mockSceneObjects.ForEach(obj => obj.SetActive(isEnteringPreviewState));
            mainViewController.importantMenuObjects.ForEach(go => go.SetActive(!isEnteringPreviewState));
            v3FlowCoordinator.isInSettingsView = isEnteringPreviewState;

            yield break;
        }

        public void RegisterEnvironmentLights()
        {
            LightWithIdMonoBehaviour[] environmentLights = lightManager.transform.parent.GetComponentsInChildren<LightWithIdMonoBehaviour>(true);

            for (int i = 0; i < environmentLights.Length; i++)
            {
                lightManager.RegisterLight(environmentLights[i]);
            }
        }

        private void SetColorForGroup(LightGroup group, Color color)
        {
            logger.Logger.Info($"Group ID: {group.groupId}; Offset: {group.startLightId}; NumberOfElements: {group.numberOfElements}");

            int offset = group.startLightId;
            int numberOfElements = group.numberOfElements;

            for (int i = offset; i < offset + numberOfElements; i++)
            {
                this.lightManager.SetColorForId(i, color);
            }
        }

        public void Dispose()
        {
            sceneToNormalizedNames.Clear();
            normalizedToSceneNames.Clear();
        }
    }
}
