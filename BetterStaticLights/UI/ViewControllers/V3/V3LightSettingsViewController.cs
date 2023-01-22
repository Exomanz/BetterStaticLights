using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.UI.FlowCoordinators;
using BetterStaticLights.UI.ViewControllers.V3.Nested;
using SiraUtil.Logging;
using System.Linq;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3settings.bsml")]
    [HotReload(RelativePathToLayout = "../../BSML/v3settings.bsml")]
    public class V3LightSettingsViewController : BSMLAutomaticViewController
    {
        [UIParams] private readonly BSMLParserParams parser;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly SiraLog logger;

        [Inject] private readonly EnvironmentSettingsV3FlowCoordinator v3FlowCoordinator;
        [Inject] private readonly DirectionalLightSettingsViewController directionalsViewController;
        [Inject] private readonly LightGroupSettingsViewController lightGroupsViewController;

        [UIAction("lightgroups-button-was-pressed")]
        public void LightGroupsButtonPress() => this.HandleSubmenuButtonWasPressed(true, "LightGroups");

        [UIAction("directionals-button-was-pressed")]
        public void DirectionalsButtonPress() => this.HandleSubmenuButtonWasPressed(true, "Directionals");

        [UIValue("should-directionals-button-be-active")]
        public bool shouldDirectionalsButtonBeActive
        {
            get => transitionHelper.directionalLights.Count > 0 || transitionHelper.gradientBackground != null!;
        }

        [Inject]
        internal void Construct(PluginConfig config)
        {
            transitionHelper.previewerDidFinishEvent += HandlePreviewerDidFinishEvent;
        }

        public void HandlePreviewerDidFinishEvent(bool state)
        {
            this.gameObject.SetActive(state);
            if (state)
                base.NotifyPropertyChanged(nameof(shouldDirectionalsButtonBeActive));
        }

        public void HandleSubmenuButtonWasPressed(bool isEnteringOptionsMenu, string submenuName = "")
        {
            if (!isEnteringOptionsMenu)
            {
                v3FlowCoordinator.ReplaceRightScreenViewController(this, AnimationType.Out);
                return;
            }

            if (submenuName == "LightGroups")
            {
                v3FlowCoordinator.ReplaceRightScreenViewController(lightGroupsViewController, AnimationType.In);
            }

            else if (submenuName == "Directionals")
            {
                v3FlowCoordinator.ReplaceRightScreenViewController(directionalsViewController, AnimationType.In);
            }
        }
    }
}
