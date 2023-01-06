using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.UI.FlowCoordinators;
using SiraUtil.Logging;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers
{
    [ViewDefinition("BetterStaticLights.UI.BSML.home.bsml")]
    [HotReload(RelativePathToLayout = @"../BSML/home.bsml")]
    public class MainBSLViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly BSLParentFlowCoordinator parentFlowCoordinator;
        [Inject] private readonly EnvironmentSettingsV2FlowCoordinator v2FlowCoordinator;
        [Inject] private readonly EnvironmentSettingsV3FlowCoordinator v3FlowCoordinator;
        [Inject] private readonly PluginConfig config;
        [Inject] private readonly SiraLog logger;
        [UIParams] private readonly BSMLParserParams parser;

        public List<GameObject> importantMenuObjects = new List<GameObject>();

        [UIComponent("v2-button")]
        internal Button v2Button;

        [UIAction("v2-button-click")]
        public void V2ButtonPress() => _OnButtonPress("V2");

        [UIComponent("v3-button")]
        internal Button v3Button;

        [UIAction("v3-button-click")]
        public void V3ButtonPress() => _OnButtonPress("V3");

        public void Start()
        {
            importantMenuObjects.Add(GameObject.Find("DefaultMenuEnvironment"));
            importantMenuObjects.Add(GameObject.Find("MenuEnvironmentCore"));
        }

        internal void _OnButtonPress(string settingsVersion)
        {
            if (settingsVersion == "V2")
            {
                parentFlowCoordinator.PresentFlowCoordinator(v2FlowCoordinator, null, AnimationDirection.Vertical);
            }
            else if (settingsVersion == "V3")
            {
                parser.EmitEvent("hide-all");
                parentFlowCoordinator.PresentFlowCoordinator(v3FlowCoordinator, null, AnimationDirection.Vertical);
            }
        }

        [UIAction("conditional-modal-launch")]
        internal void ConditionalModalLaunch()
        {
            if (config.previewerConfigurationData.isFirstTimePreviewingEver)
            {
                config.previewerConfigurationData.isFirstTimePreviewingEver = false;
                parser.EmitEvent("show-scene-load-modal");
            }
            else this._OnButtonPress("V3");
        }
    }
}
