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

        public string previouslyLoadedEnvironment = null;

        private Scene mockScene;
        private List<GameObject> mockSceneObjects = new List<GameObject>();
        private bool hasCopiedEnvironmentElements = false;

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
                    GameObject listParent = new GameObject("== BSL MOCK OBJECTS ==");
                    mockSceneObjects.Add(listParent);

                    GameObject env = null!;
                    GameObject core = null!;

                    SceneManager.LoadSceneAsync(environmentName, LoadSceneMode.Additive);
                    SceneManager.LoadSceneAsync("GameCore", LoadSceneMode.Additive);

                    yield return new WaitForSecondsRealtime(1f);

                    mockScene = SceneManager.GetSceneByName(environmentName);

                    yield return UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                    {
                        void AddNewMock(GameObject obj)
                        {
                            if (obj == null) return;

                            obj = GameObject.Instantiate(obj, mainViewController.transform.parent, true);
                            obj.name = "BSLMock - " + obj.name;
                            obj.name.TrimStart('(');
                            obj.transform.SetParent(listParent.transform);
                            mockSceneObjects.Add(obj);
                        }

                        env = Resources.FindObjectsOfTypeAll<EnvironmentSceneSetup>()[0]?.transform.parent.gameObject;
                        env.transform.SetParent(listParent.transform);

                        core = Resources.FindObjectsOfTypeAll<GameCoreSceneSetup>()[0]?.transform.parent.gameObject;
                        AddNewMock(core.transform.Find("BloomSkyboxQuad").gameObject);

                        GameObject.Destroy(env.GetComponentInChildren<CoreGameHUDController>().gameObject);
                        GameObject.Destroy(env.GetComponentInChildren<MoveAndRotateWithMainCamera>().gameObject);

                        switch (environmentName)
                        {
                            case "PyroEnvironment":
                                env.GetComponentsInChildren<FireEffect>().ToList().ForEach(obj => GameObject.Destroy(obj.gameObject));

                                break;
                            case "TheSecondEnvironment":
                                GameObject.Destroy(env.GetComponentInChildren<SmoothStepPositionGroupEventEffect>());

                                break;
                            case "LizzoEnvironment":
                                env.GetComponentsInChildren<WhiteColorOrAlphaGroupEffectManager>().ToList().ForEach(obj => GameObject.Destroy(obj.gameObject));

                                break;
                        }
                    });

                    SceneManager.UnloadSceneAsync("GameCore", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    mockSceneObjects.Add(env);
                    previouslyLoadedEnvironment = environmentName;
                    hasCopiedEnvironmentElements = true;
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
            }

            else if (destroyCachedEnvironmentObjects)
            {
                mockSceneObjects.ForEach(obj => GameObject.Destroy(obj));
                mockSceneObjects.Clear();
                hasCopiedEnvironmentElements = false;

                if (mockScene.name != null)
                    SceneManager.UnloadSceneAsync(mockScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                yield break;
            }

            mockSceneObjects.ForEach(obj => obj.SetActive(isEnteringPreviewState));
            mainViewController.importantMenuObjects.ForEach(go => go.SetActive(!isEnteringPreviewState));
            v3FlowCoordinator.isInSettingsView = isEnteringPreviewState;

            yield break;
        }

        public void Dispose()
        {
            sceneToNormalizedNames.Clear();
            normalizedToSceneNames.Clear();
        }
    }
}
