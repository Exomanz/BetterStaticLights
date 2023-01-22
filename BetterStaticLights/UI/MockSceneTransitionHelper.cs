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
using System.Threading.Tasks;
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

        public async void RefreshPreviewer(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool destroyCachedEnvironmentObjects = false)
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

        // This works... don't touch it.
        public IEnumerator SetOrChangeEnvironmentPreview(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool destroyCachedEnvironmentObjects = false)
        {
            if (destroyCachedEnvironmentObjects)
            {
                hasCopiedEnvironmentElements = false;
                mockSceneObjects.ForEach(obj =>
                {
                    GameObject.Destroy(obj);
                    mockSceneObjects.Remove(obj);
                });

                if (mockScene.name != null) SceneManager.UnloadSceneAsync(mockScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                yield break;
            }

            if (string.IsNullOrWhiteSpace(environmentName))
            {
                logger.Logger.Error($"Illegal argument given for string argument 'environmentName'. Got {environmentName}; loading 'WeaveEnvironment'");
                previewerData.environmentKey = "WeaveEnvironment";
            }

            this.previewerDidFinishEvent(false);
            ColorScheme currentColorScheme = playerData.playerData.colorSchemesSettings.GetSelectedColorScheme();

            if (isEnteringPreviewState)
            {
                if (!hasCopiedEnvironmentElements)
                {
                    GameObject root = new GameObject("BSL | Mock Object Root");
                    mockSceneObjects.Add(root);

                    GameObject environmentRoot = null!;
                    GameObject gameCoreRoot = null!;

                    SceneManager.LoadSceneAsync(environmentName, LoadSceneMode.Additive);
                    SceneManager.LoadSceneAsync("GameCore", LoadSceneMode.Additive);
                    yield return new WaitForSecondsRealtime(1f);

                    mockScene = SceneManager.GetSceneByName(environmentName);
                    environmentRoot = Resources.FindObjectsOfTypeAll<EnvironmentSceneSetup>()[0]?.transform.parent.gameObject;
                    environmentRoot.transform.SetParent(root.transform);
                    environmentRoot.name = "BSLMock - Environment";

                    gameCoreRoot = Resources.FindObjectsOfTypeAll<GameCoreSceneSetup>()[0]?.transform.parent.gameObject;
                    GameObject skybox = GameObject.Instantiate(gameCoreRoot.transform.Find("BloomSkyboxQuad").gameObject, root.transform, true);
                    skybox.name = "BSLMock - BloomSkyboxQuad";
                    mockSceneObjects.Add(skybox);

                    directionalLights.Clear();
                    gradientBackground = null!;

                    yield return UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                    {
                        // Remove Saber Sparkle Managers
                        GameObject.Destroy(environmentRoot.GetComponentInChildren<SaberBurnMarkArea>().gameObject);
                        GameObject.Destroy(environmentRoot.GetComponentInChildren<SaberBurnMarkSparkles>().gameObject);

                        // Remove HUD
                        GameObject.DestroyImmediate(environmentRoot.GetComponentInChildren<CoreGameHUDController>().gameObject);

                        // Remove exception-throwing LightSwitch managers
                        // The LightTranslationGroupEffectManager doesn't always exist, so that one has to be checked
                        environmentRoot.GetComponentsInChildren<LightSwitchEventEffect>().ToList().ForEach(effect => GameObject.Destroy(effect.gameObject));
                        GameObject.Destroy(environmentRoot.GetComponentInChildren<LightColorGroupEffectManager>().gameObject);
                        GameObject.Destroy(environmentRoot.GetComponentInChildren<LightRotationGroupEffectManager>().gameObject);

                        switch (environmentName)
                        {
                            // This environment is touchy... god forbid you do this in a different order
                            case "PyroEnvironment":
                                environmentRoot.GetComponentsInChildren<FireEffect>().ToList().ForEach(effect => GameObject.Destroy(effect.gameObject));
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SongTimeSyncedVideoPlayer>().gameObject);
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<EnvironmentStartEndSongAudioEffect>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SpectrogramRow>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SongTimeToShaderWriter>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<EnvironmentShaderWarmup>().gameObject);
                                break;

                            // Simple enough
                            case "EDMEnvironment":
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<Spectrogram>());
                                break;

                            // Cool
                            case "TheSecondEnvironment":
                                environmentRoot.GetComponentsInChildren<LightRotationEventEffect>().ToList().ForEach(effect => GameObject.Destroy(effect));
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<Spectrogram>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SmoothStepPositionGroupEventEffect>());
                                environmentRoot.GetComponentsInChildren<DirectionalLight>().ToList().ForEach(light => light.color = Color.clear);
                                break;

                            // Done
                            case "LizzoEnvironment":
                                environmentRoot.GetComponentsInChildren<WhiteColorOrAlphaGroupEffectManager>().ToList().ForEach(effect => GameObject.Destroy(effect));
                                environmentRoot.GetComponentsInChildren<ParticleSystemEmitEventEffect>().ToList().ForEach(effect => GameObject.Destroy(effect.gameObject));
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                break;

                            // Pogchamp
                            case "TheWeekndEnvironment":
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<LightTranslationGroupEffectManager>().gameObject);
                                break;

                            // Yeehaw
                            case "RockMixtapeEnvironment":
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<LightTranslationGroupEffectManager>().gameObject);
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SongTimeToShaderWriter>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<Spectrogram>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<EnvironmentStartEndSongAudioEffect>());
                                break;
                        }

                        this.directionalLights = environmentRoot.GetComponentsInChildren<DirectionalLight>().ToList();
                        this.gradientBackground = environmentRoot.transform.GetComponentInChildren<BloomPrePassBackgroundColorsGradient>();
                    });

                    previouslyLoadedEnvironment = environmentName;
                    mockSceneObjects.Add(environmentRoot);
                    hasCopiedEnvironmentElements = true;

                    SceneManager.UnloadSceneAsync("GameCore", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    environmentLightGroups.Clear();
                    environmentLightGroups = environmentRoot.GetComponentsInChildren<LightGroup>().ToList();
                    lightManager = environmentRoot.GetComponentInChildren<LightWithIdManager>();

                    LightWithIdMonoBehaviour[] lights = lightManager.transform.parent.GetComponentsInChildren<LightWithIdMonoBehaviour>(true);
                    for (int i = 0; i < lights.Length; i++)
                        lightManager.RegisterLight(lights[i]);
                }

                else if (!string.Equals(previewerData.environmentKey, previewerData))
                {
                    previouslyLoadedEnvironment = previewerData.environmentKey;
                    hasCopiedEnvironmentElements = false;

                    mockSceneObjects.ForEach(obj =>
                    {
                        GameObject.Destroy(obj);
                        mockSceneObjects.Remove(obj);
                    });

                    SceneManager.UnloadSceneAsync(mockScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                    yield return SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(true, environmentName));
                }
            }

            mockSceneObjects.ForEach(obj => obj.SetActive(isEnteringPreviewState));
            importantMenuObjects.ForEach(obj => obj.SetActive(!isEnteringPreviewState));

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

            this.previewerDidFinishEvent(true);
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
