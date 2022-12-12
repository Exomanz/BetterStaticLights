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
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
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
            get => MockSceneTransitionHelper.GetNormalizedSceneName(config.nextPreviewEnvironment);
            set => config.nextPreviewEnvironment = value;
        }

        public List<GameObject> importantMenuObjects = new List<GameObject>();

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        public void Start()
        {
            importantMenuObjects.Add(GameObject.Find("DefaultMenuEnvironment"));
            importantMenuObjects.Add(GameObject.Find("MenuEnvironmentCore"));
        }

        internal void _OnButtonPress(string settingsVersion)
        {
            switch (settingsVersion)
            {
                case "V2":
                    parentFlowCoordinator.PresentFlowCoordinator(v2FlowCoordinator, null, AnimationDirection.Vertical);
                    break;

                case "V3":
                    parser.EmitEvent("hide-all");
                    parentFlowCoordinator.PresentFlowCoordinator(v3FlowCoordinator, null, AnimationDirection.Vertical);
                    break;

                default:
                    break;
            }
        }

        [UIAction("conditional-modal-launch")]
        internal void ConditionalModalLaunch()
        {
            if (config.firstTimePreviewing)
            {
                //config.firstTimePreviewing = false;
                parser.EmitEvent("show-scene-load-modal");
            }
            else this._OnButtonPress("V3");
        }
    }
}
