using BetterStaticLights.Configuration;
using BetterStaticLights.UI.FlowCoordinators;
using BetterStaticLights.UI.ViewControllers;
using IPA.Utilities;
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
    internal class MockSceneTransitionHelper
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

        public static Dictionary<string, string> sceneToNormalizedNames { get; } = new Dictionary<string, string>()
        {
            { "WeaveEnvironment", "Weave" },
            { "PyroEnvironment", "Fall Out Boy" },
            { "EDMEnvironment", "EDM" },
            { "TheSecondEnvironment", "The Second" },
            { "LizzoEnvironment", "Lizzo"},
            { "TheWeekndEnvironment", "The Weeknd" },
            { "RockMixtapeEnvironment", "Rock Mixtape" }
        };

        public static Dictionary<string, string> normalizedToSceneNames { get; } = new Dictionary<string, string>()
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
        [Inject] private readonly PlayerDataModel playerData;
        [Inject] private readonly SpecificEnvironmentSettingsLoader environmentSettingsLoader;

        public event Action<bool> previewerDidFinishEvent = delegate { };
        public string previouslyLoadedEnvironment = null!;
        public List<LightGroup> environmentLightGroups = new List<LightGroup>(501);
        public List<DirectionalLight> directionalLights = new List<DirectionalLight>();
        public BloomPrePassBackgroundColorsGradient gradientBackground = null!;

        private SpecificEnvironmentSettingsLoader.SpecificEnvironmentSettings activelyLoadedSettings;
        private List<GameObject> mockSceneObjects = new List<GameObject>();
        private List<GameObject> importantMenuObjects = new List<GameObject>();
        private PreviewerConfigurationData previewerData;
        private bool hasCopiedEnvironmentElements = false;
        private Scene mockScene;
        private LightWithIdManager lightManager;

        [Inject] internal void Construct(StandardLevelDetailViewController sdlvc, PluginConfig config)
        {
            this.previewerData = config.PreviewerConfigurationData;
            this.importantMenuObjects.Add(GameObject.Find("DefaultMenuEnvironment"));
            this.importantMenuObjects.Add(GameObject.Find("MenuEnvironmentCore"));

            sdlvc.didPressActionButtonEvent -= this.Cleanup;
            sdlvc.didPressActionButtonEvent += this.Cleanup;
        }

        public async void Update(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool destroyCachedEnvironmentObjects = false)
        {
            if (activelyLoadedSettings != null)
            {
                await environmentSettingsLoader?.SaveEnvironmentSettings(activelyLoadedSettings);
                activelyLoadedSettings = null;
            }
            if (isEnteringPreviewState)
                activelyLoadedSettings = await environmentSettingsLoader?.LoadEnvironmentSettings(environmentName);

            SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(isEnteringPreviewState, environmentName, destroyCachedEnvironmentObjects));
        }

        public IEnumerator SetOrChangeEnvironmentPreview(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool destroyCachedEnvironmentObjects = false)
        {
            this.previewerDidFinishEvent?.Invoke(false);

            if (string.IsNullOrWhiteSpace(environmentName))
            {
                logger.Logger.Error($"Illegal argument given for string argument 'environmentName'.\nReceived: {environmentName!}; Loading 'WeaveEnvironment'");
                previewerData.environmentKey = "WeaveEnvironment";
            }

            ColorScheme currentColorScheme = playerData.playerData.colorSchemesSettings.GetColorSchemeForId(previewerData.colorSchemeKey);

            if (isEnteringPreviewState)
            {
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
                    env.transform.SetParent(listParent.transform);
                    env.name = "BSLMock - Environment";

                    core = Resources.FindObjectsOfTypeAll<GameCoreSceneSetup>()[0]?.transform.parent.gameObject;
                    GameObject skybox = GameObject.Instantiate(core.transform.Find("BloomSkyboxQuad").gameObject, listParent.transform, true);
                    skybox.name = "BSLMock - BloomSkyboxQuad";
                    mockSceneObjects.Add(skybox);

                    directionalLights.Clear();
                    gradientBackground = null!;

                    // If ANY GameObject.Destroy() method fails it throws a silent exception, the task returns early, and shit breaks
                    yield return UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                    {
                        // Remove Saber Sparkle Managers
                        GameObject.Destroy(env.GetComponentInChildren<SaberBurnMarkArea>().gameObject);
                        GameObject.Destroy(env.GetComponentInChildren<SaberBurnMarkSparkles>().gameObject);

                        // Remove HUD
                        GameObject.Destroy(env.GetComponentInChildren<CoreGameHUDController>().gameObject);

                        // Remove exception-throwing LightSwitch managers
                        // The LightTranslationGroupEffectManager doesn't always exist, so that one has to be checked
                        env.GetComponentsInChildren<LightSwitchEventEffect>().ToList().ForEach(effect => GameObject.Destroy(effect.gameObject));
                        GameObject.Destroy(env.GetComponentInChildren<LightColorGroupEffectManager>().gameObject);
                        GameObject.Destroy(env.GetComponentInChildren<LightRotationGroupEffectManager>().gameObject);

                        switch (environmentName)
                        {
                            // This environment is touchy... god forbid you do this in a different order
                            case "PyroEnvironment":
                                env.GetComponentsInChildren<FireEffect>().ToList().ForEach(effect => GameObject.Destroy(effect));
                                GameObject.Destroy(env.GetComponentInChildren<SongTimeSyncedVideoPlayer>().gameObject);
                                GameObject.Destroy(env.GetComponentInChildren<EnvironmentStartEndSongAudioEffect>());
                                GameObject.Destroy(env.GetComponentInChildren<SpectrogramRow>());
                                GameObject.Destroy(env.GetComponentInChildren<SongTimeToShaderWriter>());
                                GameObject.Destroy(env.GetComponentInChildren<EnvironmentShaderWarmup>().gameObject);
                                env.transform.Find("GradientBackgroundPyro").GetComponent<BloomPrePassBackgroundColorsGradient>().tintColor = Color.clear;
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
                                env.GetComponentsInChildren<DirectionalLight>().ToList().ForEach(light => light.color = Color.clear);
                                break;
                                
                            // Done
                            case "LizzoEnvironment":
                                env.GetComponentsInChildren<WhiteColorOrAlphaGroupEffectManager>().ToList().ForEach(effect => GameObject.Destroy(effect));
                                env.GetComponentsInChildren<ParticleSystemEmitEventEffect>().ToList().ForEach(effect => GameObject.Destroy(effect.gameObject));
                                GameObject.Destroy(env.GetComponentInChildren<MoveAndRotateWithMainCamera>());

                                env.transform.Find("GradientBackgroundLizzo").GetComponent<BloomPrePassBackgroundColorsGradient>().tintColor = Color.clear;
                                env.GetComponentsInChildren<DirectionalLight>().ToList().ForEach(light => light.color = Color.clear);
                                break;

                            // Pogchamp
                            case "TheWeekndEnvironment":
                                GameObject.Destroy(env.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                GameObject.Destroy(env.GetComponentInChildren<LightTranslationGroupEffectManager>().gameObject);
                                env.GetComponentsInChildren<DirectionalLight>().ToList().ForEach(light => light.color = Color.clear);
                                env.transform.Find("GradientBackground").GetComponent<BloomPrePassBackgroundColorsGradient>().tintColor = Color.clear;
                                break;

                            // Yeehaw
                            case "RockMixtapeEnvironment":
                                GameObject.Destroy(env.GetComponentInChildren<LightTranslationGroupEffectManager>().gameObject);
                                GameObject.Destroy(env.GetComponentInChildren<SongTimeToShaderWriter>());
                                GameObject.Destroy(env.GetComponentInChildren<Spectrogram>());
                                GameObject.Destroy(env.GetComponentInChildren<EnvironmentStartEndSongAudioEffect>());
                                env.transform.Find("GradientBackground").GetComponent<BloomPrePassBackgroundColorsGradient>().tintColor = Color.clear;
                                env.GetComponentsInChildren<DirectionalLight>().ToList().ForEach(light => light.color = Color.clear);
                                break;
                        }

                        this.directionalLights = env.GetComponentsInChildren<DirectionalLight>().ToList();
                        this.gradientBackground = env.transform.GetComponentInChildren<BloomPrePassBackgroundColorsGradient>();
                    });

                    previouslyLoadedEnvironment = environmentName;
                    mockSceneObjects.Add(env);
                    hasCopiedEnvironmentElements = true;

                    SceneManager.UnloadSceneAsync("GameCore", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    environmentLightGroups.Clear();
                    environmentLightGroups = env.GetComponentsInChildren<LightGroup>().ToList();
                    lightManager = env.GetComponentInChildren<LightWithIdManager>();

                    // Register the environment's lights so they can be modified
                    LightWithIdMonoBehaviour[] lights = lightManager.transform.parent.GetComponentsInChildren<LightWithIdMonoBehaviour>(true);
                    for (int i = 0; i < lights.Length; i++)
                        lightManager.RegisterLight(lights[i]);

                    // Set Light Colors on Initial Setup
                    for (int i = 0; i < environmentLightGroups.Count; i++)
                        this.SetColorForGroup(environmentLightGroups[i], currentColorScheme.environmentColor0);
                }

                // Reset and initialize a new preview if the list value and config value are not equal
                else if (!string.Equals(previewerData.environmentKey, previouslyLoadedEnvironment))
                {
                    mockSceneObjects.ForEach(obj => GameObject.Destroy(obj));
                    mockSceneObjects.Clear();
                    SceneManager.UnloadSceneAsync(mockScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    hasCopiedEnvironmentElements = false;
                    previouslyLoadedEnvironment = previewerData.environmentKey;

                    yield return SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(true, environmentName));
                }

                // Start the previewer like normal
                else
                {
                    // Refresh ColorScheme when the previewer is launched with the objects already cached
                    for (int i = 0; i < environmentLightGroups.Count; i++) 
                        this.SetColorForGroup(environmentLightGroups[i], currentColorScheme.environmentColor0);
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
            importantMenuObjects.ForEach(go => go.SetActive(!isEnteringPreviewState));

            // litreally WHAT the FUCK
            // The ColorArrayLightWithIds' _colorsArray field is RESET when the ONENABLE METHOD IS CALLED.
            // POGCHAMP I GUESS?
            // For what its worth, this works and uses less memory... :face_vomiting:
            if (isEnteringPreviewState && environmentName == "RockMixtapeEnvironment")
            {
                ColorArrayLightWithIds mountainParent = GameObject.Find("Mountains").GetComponent<ColorArrayLightWithIds>();
                Vector4[] colorsArray = mountainParent.GetField<Vector4[], ColorArrayLightWithIds>("_colorsArray");
                Vector4[] newArray = new Vector4[colorsArray.Length];

                for (int i = 0; i < newArray.Length; i++)
                {
                    newArray[i] = new Vector4(
                        currentColorScheme.environmentColor0.r,
                        currentColorScheme.environmentColor0.g,
                        currentColorScheme.environmentColor0.b,
                        currentColorScheme.environmentColor0.a
                        );
                }

                mountainParent.SetField("_colorsArray", newArray);
                mountainParent.SetColorDataToShader();
            }

            this.previewerDidFinishEvent.Invoke(true);
            yield break;
        }

        private void SetColorForGroup(LightGroup group, Color color)
        {
            int offset = group.startLightId;
            int numberOfElements = group.numberOfElements;

            for (int i = offset; i < offset + numberOfElements; i++)
                this.lightManager.SetColorForId(i, color);
        }

        private void Cleanup(StandardLevelDetailViewController _)
        {
            SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(false, destroyCachedEnvironmentObjects: true));
        }
    }
}
