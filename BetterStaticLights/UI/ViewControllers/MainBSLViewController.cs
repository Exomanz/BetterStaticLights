using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
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
        [Inject] private readonly GameScenesManager gameScenesManager;

        [Inject] private readonly EnvironmentSettingsV2FlowCoordinator v2FlowCoordinator;

        [Inject] private readonly SiraLog logger;

        [UIComponent("v2-button")]
        internal Button v2Button { get; }

        [UIComponent("v3-button")]
        internal Button v3Button { get; }

        [UIAction("v2-button-click")]
        public void V2ButtonPress() => _OnButtonPress("V2");

        [UIAction("v3-button-click")]
        public void V3ButtonPress() => _OnButtonPress("V3");

        internal void _OnButtonPress(string settingsVersion)
        {
            logger.Info(settingsVersion);

            switch (settingsVersion)
            {
                case "V2":
                    parentFlowCoordinator.PresentFlowCoordinator(v2FlowCoordinator, null, AnimationDirection.Vertical);
                    break;

                case "V3":
                    break;

                default:
                    break;
            }
        }
    }
}
