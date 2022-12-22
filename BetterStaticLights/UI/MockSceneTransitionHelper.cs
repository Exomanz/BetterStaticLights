using BetterStaticLights.UI.ViewControllers;
using SiraUtil.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;
using BetterStaticLights.UI.FlowCoordinators;
using System.Linq;
using BetterStaticLights.UI.ViewControllers.V3;
using IPA.Utilities.Async;

namespace BetterStaticLights.UI
{
    internal class MockSceneTransitionHelper : IInitializable, IDisposable
    {
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

        [Inject] private readonly SiraLog logger;
        [Inject] private readonly MainBSLViewController mainViewController;
        [Inject] private readonly EnvironmentSettingsV3FlowCoordinator v3FlowCoordinator;
        [Inject] private readonly V3ActiveSceneSettingsMenu v3SceneSettings;
        [Inject] private readonly PluginConfig config;
        [Inject] private readonly StandardLevelDetailViewController levelView;

        private List<GameObject> mockSceneObjects = new List<GameObject>();
        private bool hasCopiedEnvironmentElements = false;
        private string previouslyLoadedEnvironment = null;

        public void Initialize()
        {
            levelView.didPressActionButtonEvent += this.Cleanup;
        }

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

        public void Cleanup(StandardLevelDetailViewController sldvc)
        {
            SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(false, destroyCachedEnvironmentObjects: true));
        }

        public IEnumerator SetOrChangeEnvironmentPreview(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool destroyCachedEnvironmentObjects = false)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
            {
                config.environmentPreview = "WeaveEnvironment";
                throw new ArgumentException($"Illegal argument given for string argument 'environmentName'. Defaulting to 'WeaveEnvironment'", environmentName);
            }

            if (isEnteringPreviewState)
            {
                if (!hasCopiedEnvironmentElements)
                {
                    GameObject listParent;
                    if (GameObject.Find("== BSL MOCK OBJECTS ==") == null)
                        listParent = new GameObject("== BSL MOCK OBJECTS ==");
                    else listParent = GameObject.Find("== BSL MOCK OBJECTS ==");

                    GameObject environmentSceneWrapper = null!;
                    GameObject gameCoreSceneWrapper = null!;

                    AsyncOperation envOp = SceneManager.LoadSceneAsync(environmentName, LoadSceneMode.Additive);
                    AsyncOperation gameCoreOp = SceneManager.LoadSceneAsync("GameCore", LoadSceneMode.Additive);

                    yield return new WaitForSecondsRealtime(1f);

                    yield return UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                    {
                        environmentSceneWrapper = Resources.FindObjectsOfTypeAll<EnvironmentSceneSetup>()[0]?.transform.parent.gameObject;
                        gameCoreSceneWrapper = Resources.FindObjectsOfTypeAll<GameCoreSceneSetup>()[0]?.transform.parent.gameObject;

                        GameObject mockSkyboxBloom = gameCoreSceneWrapper.transform.Find("BloomSkyboxQuad").gameObject;
                        mockSkyboxBloom = GameObject.Instantiate(mockSkyboxBloom.gameObject, mainViewController.transform.parent, true);
                        mockSkyboxBloom.gameObject.name = "BSLMock - BloomSkyboxQuad";
                        mockSkyboxBloom.transform.SetParent(listParent.transform);
                        mockSceneObjects.Add(mockSkyboxBloom);

                        GameObject.Destroy(environmentSceneWrapper.GetComponentInChildren<SaberBurnMarkArea>());
                        GameObject.Destroy(environmentSceneWrapper.GetComponentInChildren<SaberBurnMarkSparkles>());

                        switch (environmentName)
                        {
                            case "PyroEnvironment":
                                environmentSceneWrapper.GetComponentsInChildren<FireEffect>().ToList().ForEach(obj =>
                                    GameObject.Destroy(obj));

                                break;
                            case "TheSecondEnvironment":
                                GameObject.Destroy(environmentSceneWrapper.GetComponentInChildren<SmoothStepPositionGroupEventEffect>());

                                break;
                            case "LizzoEnvironment":
                                environmentSceneWrapper.GetComponentsInChildren<WhiteColorOrAlphaGroupEffectManager>().ToList().ForEach(obj =>
                                    GameObject.Destroy(obj));

                                break;

                            /// TODO: 
                            /// Add environment-specific mock objects so that the previews are more accurate to the game.
                            /// Figure out why the PlayersPlace shaders aren't working the right way
                        }

                        foreach (LightGroup group in environmentSceneWrapper.GetComponentsInChildren<LightGroup>())
                        {
                            GameObject mockGroup = GameObject.Instantiate(group.gameObject, mainViewController.transform.parent, true);
                            mockGroup.name = $"BSLMock - {group.name}";
                            mockGroup.transform.SetParent(listParent.transform);

                            mockSceneObjects.Add(mockGroup);
                        }

                        GameObject mockPlayersPlace = environmentSceneWrapper.transform.Find("PlayersPlace")?.gameObject;
                        for (int i = 0; i < mockPlayersPlace.transform.childCount; i++)
                        {
                            GameObject playersPlaceObject = GameObject.Instantiate(mockPlayersPlace.transform.GetChild(i).gameObject);
                            playersPlaceObject.name = "BSLMock - " + playersPlaceObject.name.Trim('(');
                            playersPlaceObject.transform.SetParent(listParent.transform);

                            mockSceneObjects.Add(playersPlaceObject);
                        }
                    });

                    SceneManager.UnloadSceneAsync("GameCore", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                    SceneManager.UnloadSceneAsync(environmentName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    previouslyLoadedEnvironment = environmentName;
                    hasCopiedEnvironmentElements = true;
                }

                else if (!string.Equals(config.environmentPreview, previouslyLoadedEnvironment))
                {
                    previouslyLoadedEnvironment = config.environmentPreview;
                    mockSceneObjects.ForEach(obj => GameObject.Destroy(obj));
                    mockSceneObjects.Clear();

                    hasCopiedEnvironmentElements = false;

                    yield return SharedCoroutineStarter.instance.StartCoroutine(this.SetOrChangeEnvironmentPreview(true, environmentName));
                }
            }

            else if (destroyCachedEnvironmentObjects)
            {
                mockSceneObjects.ForEach(obj => GameObject.Destroy(obj));
                mockSceneObjects.Clear();
                hasCopiedEnvironmentElements = false;

                yield return default;
            }

            mockSceneObjects.ForEach(obj => obj.SetActive(isEnteringPreviewState));
            mainViewController.importantMenuObjects.ForEach(go => go.SetActive(!isEnteringPreviewState));
            v3FlowCoordinator.isInSettingsView = isEnteringPreviewState;

            yield return default;
        }

        public void Dispose()
        {
            sceneToNormalizedNames.Clear();
            normalizedToSceneNames.Clear();
        }
    }
}
