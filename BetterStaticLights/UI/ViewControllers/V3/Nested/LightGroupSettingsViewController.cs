using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using SiraUtil.Logging;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3.Nested
{
    [ViewDefinition("BetterStaticLights.UI.BSML.NestedV3.lightgroups.bsml")]
    [HotReload(RelativePathToLayout = "../../../BSML/NestedV3/lightgroups.bsml")]
    public class LightGroupSettingsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly SiraLog logger;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly V3LightSettingsViewController lightSettingsViewController;

        [UIValue("lightgroup-list-setting-dropdown")]
        public DropDownListSetting dropdown;

        [UIValue("lightgroup-list-choices")]
        public List<object> choices = new List<object>() { "0" };

        [UIValue("lightgroup-list-value")]
        public string value;

        [UIAction("back-button-was-pressed")]
        public void HandleBackButtonWasPressed()
        {
            lightSettingsViewController.HandleSubmenuButtonWasPressed(false);
        }

        [Inject] internal void Construct()
        {
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            choices = Enumerable.Range(0, transitionHelper.environmentLightGroups.Count).Cast<object>().ToList();
            dropdown.values = choices;
            dropdown.UpdateChoices();
        }
    }
}
