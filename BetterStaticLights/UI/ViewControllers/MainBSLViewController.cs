using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.UI.FlowCoordinators;
using IPA.Utilities.Async;
using SiraUtil.Logging;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers
{
    [ViewDefinition("BetterStaticLights.UI.BSML.home.bsml")]
    [HotReload(RelativePathToLayout = @"../BSML/home.bsml")]
    public class MainBSLViewController : BSMLAutomaticViewController
    {
        internal class MockSceneTransitionHelper : IDisposable
        {
            internal Dictionary<string, string> sceneToNormalizedNames = new Dictionary<string, string>()
            {
                { "WeaveEnvironment", "Weave" },
                { "PyroEnvironment", "Fall Out Boy" },
                { "EDMEnvironment", "EDM" },
                { "TheSecondEnvironment", "The Second" },
                { "LizzoEnvironment", "Lizzo"},
                { "TheWeekndEnvironment", "The Weeknd" },
            };

            internal Dictionary<string, string> normalizedToSceneNames = new Dictionary<string, string>()
            {
                { "Weave", "WeaveEnvironment" },
                { "Fall Out Boy", "PyroEnvironment" },
                { "EDM", "EDMEnvironment" },
                { "The Second", "TheSecondEnvironment" },
                { "Lizzo", "LizzoEnvironment" },
                { "The Weeknd", "TheWeekndEnvironment" },
            };

            private SiraLog logger = null;
            private MainBSLViewController mainViewController = null;
            private List<GameObject> mockSceneObjects = new List<GameObject>();
            private bool hasCopiedEnvironmentElements = false;

            public MockSceneTransitionHelper(SiraLog logger, MainBSLViewController viewControllerInstance)
            {
                this.logger = logger;
                this.mainViewController = viewControllerInstance;
            }

            public string GetNormalizedSceneName(string sceneName)
            {
                sceneToNormalizedNames.TryGetValue(sceneName, out string value);
                return value;
            }

            public string GetSerializableSceneName(string sceneName)
            {
                normalizedToSceneNames.TryGetValue(sceneName, out string value);
                return value;
            }

            public IEnumerator EnvironmentPreviewRoutine(bool isEnteringPreviewState, string environmentName = "WeaveEnvironment", bool shouldEnvironmentChange = false)
            {
                logger.Logger.Info($"{isEnteringPreviewState}, {environmentName}, {shouldEnvironmentChange}");

                if (string.IsNullOrWhiteSpace(environmentName))
                {
                    mainViewController.config.nextPreviewEnvironment = "WeaveEnvironment";
                    throw new ArgumentException($"Illegal argument given for {environmentName}. Defaulting to 'WeaveEnvironment'", environmentName);
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

                        int easterEggInt = 0;

                        AsyncOperation envOp = SceneManager.LoadSceneAsync(environmentName, LoadSceneMode.Additive);
                        AsyncOperation gameCoreOp = SceneManager.LoadSceneAsync("GameCore", LoadSceneMode.Additive);
                        while (!gameCoreOp.isDone && !envOp.isDone)
                        {
                            if (easterEggInt == 0)
                                logger.Logger.Debug("Hey! You found a secret debug message because I needed to add just a *little* bit more processing time before the 'WaitForFixedUpdate()' calls. Seriously! If I omit this log, the method won't work properly and the transition will break. That's why this is here, so consider this a little Easter Egg from me.");
                            easterEggInt++;
                            yield return new WaitForFixedUpdate();
                        }

                        environmentSceneWrapper = Resources.FindObjectsOfTypeAll<EnvironmentSceneSetup>()[0]?.transform.parent.gameObject;
                        gameCoreSceneWrapper = Resources.FindObjectsOfTypeAll<GameCoreSceneSetup>()[0]?.transform.parent.gameObject;

                        GameObject mockSkyboxBloom = gameCoreSceneWrapper.transform.Find("BloomSkyboxQuad").gameObject;
                        mockSkyboxBloom = Instantiate(mockSkyboxBloom.gameObject, mainViewController.transform.parent, true);
                        mockSkyboxBloom.gameObject.name = "BSLMock - BloomSkyboxQuad";
                        mockSkyboxBloom.transform.SetParent(listParent.transform);
                        mockSceneObjects.Add(mockSkyboxBloom);

                        switch (environmentName)
                        {
                            /// TODO: 
                            /// Add environment-specific mock objects so that the previews are more accurate to the game.
                            /// Figure out why the PlayersPlace shaders aren't working the right way
                            /// Add a 1-second timeout on Confirmation to load everything and prevent exceptions (this happens a lot more than it should).
                            /// Clean up... but holy fuck I'm glad this works.
                        }

                        foreach (LightGroup group in environmentSceneWrapper.GetComponentsInChildren<LightGroup>())
                        {
                            GameObject mockGroup = Instantiate(group.gameObject, mainViewController.transform.parent, true);
                            mockGroup.name = $"BSLMock - {group.name}";
                            mockGroup.transform.SetParent(listParent.transform);

                            mockSceneObjects.Add(mockGroup);
                        }

                        GameObject mockPlayersPlace = environmentSceneWrapper.transform.Find("PlayersPlace")?.gameObject;
                        for (int i = 0; i < mockPlayersPlace.transform.childCount; i++)
                        {
                            GameObject playersPlaceObject = Instantiate(mockPlayersPlace.transform.GetChild(i).gameObject);
                            playersPlaceObject.name = "BSLMock - " + playersPlaceObject.name.Trim('(');
                            playersPlaceObject.transform.SetParent(listParent.transform);

                            mockSceneObjects.Add(playersPlaceObject);
                        }

                        SceneManager.UnloadSceneAsync("GameCore", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                        SceneManager.UnloadSceneAsync(environmentName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                        this.hasCopiedEnvironmentElements = true;
                    }
                    else if (shouldEnvironmentChange)
                    {
                        mockSceneObjects.ForEach(obj => Destroy(obj));
                        mockSceneObjects.Clear();
                        this.hasCopiedEnvironmentElements = false;

                        yield return SharedCoroutineStarter.instance.StartCoroutine(EnvironmentPreviewRoutine(true, environmentName));
                    }
                }

                mockSceneObjects.ForEach(obj => obj.SetActive(isEnteringPreviewState));
                mainViewController.importantMenuObjects.ForEach(go => go.SetActive(!isEnteringPreviewState));
                mainViewController.v3FlowCoordinator.isInSettingsView = isEnteringPreviewState;

                yield return default;
            }

            public void Dispose()
            {
                sceneToNormalizedNames.Clear();
                normalizedToSceneNames.Clear();
            }
        }

        [Inject] private readonly BSLParentFlowCoordinator parentFlowCoordinator;
        [Inject] private readonly PlayerDataModel dataModel;
        [Inject] private readonly EnvironmentSettingsV2FlowCoordinator v2FlowCoordinator;
        [Inject] private readonly EnvironmentSettingsV3FlowCoordinator v3FlowCoordinator;
        [Inject] private readonly SiraLog logger;
        [Inject] private readonly PluginConfig config;

        [UIParams] private readonly BSMLParserParams parser;

        [UIComponent("v2-button")]
        internal Button v2Button;

        [UIComponent("v3-button")]
        internal Button v3Button;

        [UIComponent("v3-list")]
        internal ListSetting v3List;

        [UIAction("v2-button-click")]
        public void V2ButtonPress() => _OnButtonPress("V2");

        [UIAction("v3-button-click")]
        public void V3ButtonPress() => _OnButtonPress("V3");

        [UIValue("v3-environment-list")]
        public List<object> v3Environments { get; } = new List<object>()
        {
            "Weave",
            "Fall Out Boy",
            "EDM",
            "The Second",
            "Lizzo",
            "The Weeknd",
        };

        [UIValue("temp-value")]
        public string environmentLoadString
        {
            get
            {
                string value;
                try
                {
                    value = transitionHelper?.GetNormalizedSceneName(config.nextPreviewEnvironment);
                }
                catch
                {
                    value = transitionHelper.normalizedToSceneNames[config.nextPreviewEnvironment];
                }

                return value;
            }
            set => config.nextPreviewEnvironment = value;
        }

        private List<GameObject> importantMenuObjects = new List<GameObject>();
        internal MockSceneTransitionHelper transitionHelper = null;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (transitionHelper != null) 
                transitionHelper.Dispose();

            transitionHelper = new MockSceneTransitionHelper(this.logger, this);
        }

        public void Start()
        {
            importantMenuObjects.Add(GameObject.Find("DefaultMenuEnvironment"));
            importantMenuObjects.Add(GameObject.Find("MenuEnvironmentCore"));
        }

        internal void _OnButtonPress(string settingsVersion)
        {
            logger.Info(settingsVersion);

            switch (settingsVersion)
            {
                case "V2":
                    parentFlowCoordinator.PresentFlowCoordinator(v2FlowCoordinator, null, AnimationDirection.Vertical);
                    break;

                case "V3":
                    string listValue = v3List.Value?.ToString();
                    string serializedName = transitionHelper.GetSerializableSceneName(listValue);
                    
                    // If the environment hasn't changed since last time, don't load it again.
                    bool shouldEnvironmentBeChanged = !string.Equals(config.nextPreviewEnvironment, serializedName);
                    environmentLoadString = serializedName;

                    logger.Logger.Info($"Environment to load: {config.nextPreviewEnvironment} | Should Environment Change: {shouldEnvironmentBeChanged}");

                    parser.EmitEvent("hide-all");
                    UnityMainThreadTaskScheduler.Factory.StartNew(() => base.StartCoroutine(transitionHelper?.EnvironmentPreviewRoutine(true, config.nextPreviewEnvironment, shouldEnvironmentBeChanged)));
                    parentFlowCoordinator.PresentFlowCoordinator(v3FlowCoordinator, null, AnimationDirection.Vertical);
                    break;

                default:
                    break;
            }
        }
    }
}
