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
            get => sceneToNormalizedEnvironmentNames[config.nextPreviewEnvironment];
            set => config.nextPreviewEnvironment = value;
        }

        private Dictionary<string, string> sceneToNormalizedEnvironmentNames = new Dictionary<string, string>()
        {
            { "WeaveEnvironment", "Weave" },
            { "PyroEnvironment", "Fall Out Boy" },
            { "EDMEnvironment", "EDM" },
            { "TheSecondEnvironment", "The Second" },
            { "LizzoEnvironment", "Lizzo"},
            { "TheWeekndEnvironment", "The Weeknd" },
        };

        private Dictionary<string, string> normalizedToSceneEnvironmentNames = new Dictionary<string, string>()
        {
            { "Weave", "WeaveEnvironment" },
            { "Fall Out Boy", "PyroEnvironment" },
            { "EDM", "EDMEnvironment" },
            { "The Second", "TheSecondEnvironment" },
            { "Lizzo", "LizzoEnvironment" },
            { "The Weeknd", "TheWeekndEnvironment" },
        };

        private List<GameObject> mockSceneObjects = new List<GameObject>();
        private List<GameObject> importantMenuObjects = new List<GameObject>();
        private bool hasCopiedEnvironmentElements = false;

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
                    string normalizedEnvironmentValue = v3List.Value?.ToString();
                    bool shouldEnvironmentBeChanged = !string.Equals(config.nextPreviewEnvironment, normalizedToSceneEnvironmentNames[normalizedEnvironmentValue]);
                    environmentLoadString = normalizedToSceneEnvironmentNames[normalizedEnvironmentValue];

                    logger.Logger.Info($"Environment to load: {config.nextPreviewEnvironment} | Should Environment Change: {shouldEnvironmentBeChanged}");

                    parser.EmitEvent("hide-all");
                    UnityMainThreadTaskScheduler.Factory.StartNew(() => base.StartCoroutine(this.EnvironmentPreviewRoutine(true, config.nextPreviewEnvironment, shouldEnvironmentBeChanged)));
                    parentFlowCoordinator.PresentFlowCoordinator(v3FlowCoordinator, null, AnimationDirection.Vertical);
                    break;

                default:
                    break;
            }
        }

        internal IEnumerator EnvironmentPreviewRoutine(bool isEnteringPreviewState, string environmentName, bool shouldEnvironmentChange = false)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
            {
                throw new ArgumentException(environmentName);
            }

            if (isEnteringPreviewState)
            {
                if (!this.hasCopiedEnvironmentElements)
                {
                    GameObject listParent = new GameObject("== BSL MOCK OBJECTS ==");
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

                    logger.Info($"EnvWrapper = {environmentSceneWrapper?.name}; GameCoreWrapper = {gameCoreSceneWrapper?.name}");

                    GameObject mockSkyboxBloom = gameCoreSceneWrapper.transform.Find("BloomSkyboxQuad").gameObject;
                    mockSkyboxBloom = Instantiate(mockSkyboxBloom.gameObject, this.transform.parent, true);
                    mockSkyboxBloom.gameObject.name = "BSLMock - BloomSkyboxQuad";
                    mockSkyboxBloom.transform.SetParent(listParent.transform);
                    mockSceneObjects.Add(mockSkyboxBloom);

                    switch (environmentName)
                    {
                        /// TODO: 
                        /// Add environment-specific mock objects so that the previews are more accurate to the game.
                        /// Figure out why the PlayersPlace shaders aren't working the right way
                        /// MAYBE: Add a 1-second timeout on Confirmation to load everything and prevent exceptions.
                        /// Clean up... but holy fuck I'm glad this works.
                    }

                    foreach (LightGroup group in environmentSceneWrapper.GetComponentsInChildren<LightGroup>())
                    {
                        GameObject mockGroup = Instantiate(group.gameObject, this.transform.parent, true);
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

                    yield return StartCoroutine(EnvironmentPreviewRoutine(true, environmentName));
                }
            }

            mockSceneObjects.ForEach(obj => obj.SetActive(isEnteringPreviewState));
            importantMenuObjects.ForEach(go => go.SetActive(!isEnteringPreviewState));
            v3FlowCoordinator.isInSettingsView = isEnteringPreviewState;

            yield return default;
        }
    }
}
