using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.UI.FlowCoordinators;
using SiraUtil.Logging;
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

        [UIComponent("v2-button")]
        internal Button v2Button;

        [UIAction("v2-button-click")]
        public void V2ButtonPress() => _OnButtonPress("V2");

        [UIComponent("v3-button")]
        internal Button v3Button;

        [UIAction("v3-button-click")]
        public void V3ButtonPress() => _OnButtonPress("V3");

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
            if (config.PreviewerConfigurationData.isFirstTimePreviewingEver)
            {
                config.PreviewerConfigurationData.isFirstTimePreviewingEver = false;
                parser.EmitEvent("show-scene-load-modal");
            }
            else this._OnButtonPress("V3");
        }
    }
}
