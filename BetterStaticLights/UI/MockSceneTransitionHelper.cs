using BetterStaticLights.Configuration;
using BetterStaticLights.UI.ViewControllers;
using HarmonyLib;
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
            "Rock Mixtape",
            "Dragons 2.0"
        };

        public static Dictionary<string, string> sceneToNormalizedNames { get; } = new Dictionary<string, string>()
        {
            { "WeaveEnvironment", "Weave" },
            { "PyroEnvironment", "Fall Out Boy" },
            { "EDMEnvironment", "EDM" },
            { "TheSecondEnvironment", "The Second" },
            { "LizzoEnvironment", "Lizzo"},
            { "TheWeekndEnvironment", "The Weeknd" },
            { "RockMixtapeEnvironment", "Rock Mixtape" },
            { "Dragons2Environment", "Dragons 2.0" }
        };

        public static Dictionary<string, string> normalizedToSceneNames { get; } = new Dictionary<string, string>()
        {
            { "Weave", "WeaveEnvironment" },
            { "Fall Out Boy", "PyroEnvironment" },
            { "EDM", "EDMEnvironment" },
            { "The Second", "TheSecondEnvironment" },
            { "Lizzo", "LizzoEnvironment" },
            { "The Weeknd", "TheWeekndEnvironment" },
            { "Rock Mixtape", "RockMixtapeEnvironment" },
            { "Dragons 2.0", "Dragons2Environment" }
        };

        public static string GetNormalizedSceneName(string sceneName)
        {
            sceneToNormalizedNames.TryGetValue(sceneName, out string value);
            return value ?? string.Empty;
        }

        public static string GetSerializableSceneName(string sceneName)
        {
            normalizedToSceneNames.TryGetValue(sceneName, out string value);
            return value ?? string.Empty;
        }
        #endregion

        [Inject] private readonly SiraLog logger;
        [Inject] private readonly MainBSLViewController mainViewController;
        [Inject] private readonly PlayerDataModel playerData;
        [Inject] private readonly SpecificEnvironmentSettingsLoader environmentSettingsLoader;

        public event Action<bool> previewerDidFinishEvent = delegate { };
        public string previouslyLoadedEnvironment = null!;
        public List<int> ignoredLightGroups = new List<int>(501);
        public List<LightGroup> environmentLightGroups = new List<LightGroup>(501);
        public List<DirectionalLight> directionalLights = new List<DirectionalLight>();
        public BloomPrePassBackgroundColorsGradient gradientBackground = null!;
        public SpecificEnvironmentSettingsLoader.SpecificEnvironmentSettings activelyLoadedSettings;
        public FieldAccessor<ColorArrayLightWithIds, Vector4[]>.Accessor colorsArrayAccessor = FieldAccessor<ColorArrayLightWithIds, Vector4[]>.GetAccessor("_colorsArray");

        private List<GameObject> mockSceneObjects = new List<GameObject>();
        private List<GameObject> importantMenuObjects = new List<GameObject>();
        private PreviewerConfigurationData previewerData;
        private bool hasCopiedEnvironmentElements = false;
        private Scene mockScene;
        private LightWithIdManager lightManager;

        [Inject]
        internal void Construct(StandardLevelDetailViewController sdlvc, PluginConfig config)
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

        public IEnumerator SetOrChangeEnvironmentPreview(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool destroyCachedEnvironmentObjects = false)
        {
            if (destroyCachedEnvironmentObjects)
            {
                hasCopiedEnvironmentElements = false;
                mockSceneObjects.ForEach(obj =>
                {
                    GameObject.Destroy(obj);
                });
                mockSceneObjects.Clear();

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
                    ignoredLightGroups.Clear();
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
                                environmentRoot.GetComponentsInChildren<FireEffect>().ToList().ForEach(effectObj => GameObject.Destroy(effectObj.gameObject));
                                GameObject.Destroy(environmentRoot.transform.Find("Fire").gameObject);
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<EnvironmentShaderWarmup>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SongTimeToShaderWriter>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SongTimeSyncedVideoPlayer>().gameObject);
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<SpectrogramRow>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<EnvironmentStartEndSongAudioEffect>());

                                ignoredLightGroups.Add(6);
                                ignoredLightGroups.Add(7);
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

                            case "Dragons2Environment":
                                environmentRoot.GetComponentsInChildren<ParticleSystemLightWithId>().ToList().ForEach(effect => GameObject.DestroyImmediate(effect));
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<MoveAndRotateWithMainCamera>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<Spectrogram>());
                                GameObject.Destroy(environmentRoot.GetComponentInChildren<LightTranslationGroupEffectManager>());

                                ignoredLightGroups.Add(7);
                                ignoredLightGroups.Add(8);
                                break;
                        }

                        this.directionalLights = environmentRoot.GetComponentsInChildren<DirectionalLight>().ToList();
                        this.gradientBackground = environmentRoot.transform.GetComponentInChildren<BloomPrePassBackgroundColorsGradient>();
                    });

                    SceneManager.UnloadSceneAsync("GameCore", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    lightManager = environmentRoot.GetComponentInChildren<LightWithIdManager>();
                    environmentLightGroups.Clear();
                    environmentLightGroups = environmentRoot.GetComponentsInChildren<LightGroup>(true).ToList();

                    LightWithIdMonoBehaviour[] lights = lightManager.transform.parent.GetComponentsInChildren<LightWithIdMonoBehaviour>(true);
                    for (int i = 0; i < lights.Length; i++)
                    {
                        logger.Info(lights[i].lightId);
                        lightManager.RegisterLight(lights[i]);
                    }

                    previouslyLoadedEnvironment = environmentName;
                    mockSceneObjects.Add(environmentRoot);
                    hasCopiedEnvironmentElements = true;
                }

                else if (!string.Equals(previewerData.environmentKey, previouslyLoadedEnvironment))
                {
                    previouslyLoadedEnvironment = previewerData.environmentKey;
                    hasCopiedEnvironmentElements = false;

                    mockSceneObjects.ForEach(obj =>
                    {
                        GameObject.Destroy(obj);
                    });
                    mockSceneObjects.Clear();

                    SceneManager.UnloadSceneAsync(mockScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                    yield return SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(true, environmentName));
                }
            }

            mockSceneObjects.ForEach(obj => obj.SetActive(isEnteringPreviewState));
            importantMenuObjects.ForEach(obj => obj.SetActive(!isEnteringPreviewState));

            if (isEnteringPreviewState)
            {
                // I have to do this after showing the objects because the OnEnable() method in the ColorArrayLightWithIds resets the array I need to modify.
                if (environmentName == "RockMixtapeEnvironment")
                {
                    ColorArrayLightWithIds mountainParent = GameObject.Find("Mountains").GetComponent<ColorArrayLightWithIds>();
                    Vector4[] colorsArray = colorsArrayAccessor(ref mountainParent);
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

                for (int i = 0; i < environmentLightGroups.Count; i++)
                {
                    Color colorForGroup = activelyLoadedSettings.LightGroupSettings[i].GroupColor;
                    colorForGroup = colorForGroup.ColorWithAlpha(activelyLoadedSettings.LightGroupSettings[i].Brightness);
                    this.SetColorForGroup(environmentLightGroups[i], colorForGroup);
                }
            }

            this.previewerDidFinishEvent(isEnteringPreviewState);
            yield break;
        }

        public void SetColorForGroup(LightGroup group, Color color)
        {
            int offset = group.startLightId;
            int numberOfElements = group.numberOfElements;

            for (int i = offset; i < offset + numberOfElements; i++)
            {
                this.lightManager.SetColorForId(i, color);
            }
        }

        private void Cleanup(StandardLevelDetailViewController _)
        {
            SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(false, destroyCachedEnvironmentObjects: true));
        }
    }
}
