using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.Configuration;
using BetterStaticLights.UI.FlowCoordinators;
using HMUI;
using IPA.Utilities;
using SiraUtil.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tweening;
using UnityEngine;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3settings.bsml")]
    [HotReload(RelativePathToLayout = "../../BSML/v3settings.bsml")]
    public class V3LightSettingsViewController : BSMLAutomaticViewController
    {
        [UIParams] private readonly BSMLParserParams parser;
        [Inject] private readonly SpecificEnvironmentSettingsLoader specificEnvironmentSettingsLoader;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly TimeTweeningManager tweeningManager;
        [Inject] private readonly SiraLog logger;

        [Inject] private readonly EnvironmentSettingsV3FlowCoordinator v3FlowCoordinator;

        private ColorTween selectionTween = null!;
        private SpecificEnvironmentSettingsLoader.SpecificEnvironmentSettings activeEnvironmentSettings = null!;

        #region BSML
        [UIComponent("light-group-component")]
        public DropDownListSetting groupListSettingComponent;

        [UIValue("light-group-value")]
        public string groupId = "0";

        private int _groupId = 0;

        [UIValue("light-group-choices")]
        public List<object> groupChoices = new List<object>() { "0" };

        [UIComponent("group-color-setting")]
        public ColorSetting groupColorSettingComponent;

        [UIValue("color-for-group-id")]
        public Color colorForGroupId
        {
            get => activeEnvironmentSettings.LightGroupSettings[_groupId].GroupColor;
            set
            {
                activeEnvironmentSettings.LightGroupSettings[_groupId].GroupColor = value;
                transitionHelper.SetColorForGroup(transitionHelper.environmentLightGroups[_groupId], value.ColorWithAlpha(activeEnvironmentSettings.LightGroupSettings[_groupId].Brightness));

                base.NotifyPropertyChanged();
            }
        }

        [UIAction("update-view-for-group")]
        internal void UpdateViewForGroup(int index)
        {
            SpecificEnvironmentSettingsLoader.LightGroupSettings groupSettings = activeEnvironmentSettings.LightGroupSettings[index];
            if (groupSettings != null)
            {
                groupId = groupSettings.GroupId.ToString();
                this._groupId = groupSettings.GroupId;

                Color groupColorForAnimation = activeEnvironmentSettings.LightGroupSettings[_groupId].GroupColor;
                Color finalColor = groupColorForAnimation.ColorWithAlpha(activeEnvironmentSettings.LightGroupSettings[_groupId].Brightness);
                tweeningManager.RestartTween(new ColorTween(Color.red, finalColor, delegate (Color color)
                {
                    transitionHelper.SetColorForGroup(transitionHelper.environmentLightGroups[_groupId], color);
                }, 1f, EaseType.InOutSine), this);

                parser.EmitEvent("refresh-view");
            }
        }

        [UIComponent("group-brightness-component")]
        public SliderSetting groupBrightnessComponent;

        [UIValue("brightness-for-group-id")]
        public float groupBrightness
        {
            get => activeEnvironmentSettings.LightGroupSettings[_groupId].Brightness;
            set
            {
                this.activeEnvironmentSettings.LightGroupSettings[_groupId].Brightness = value;

                if (transitionHelper.environmentLightGroups[_groupId] != null)
                    transitionHelper.SetColorForGroup(transitionHelper.environmentLightGroups[_groupId], activeEnvironmentSettings.LightGroupSettings[_groupId].GroupColor.ColorWithAlpha(value));

                base.NotifyPropertyChanged();
            }
        }
        #endregion

        [Inject]
        internal void Construct(PluginConfig config)
        {
            transitionHelper.previewerDidFinishEvent += HandlePreviewerDidFinishEvent;

            // This needs to have a value that's not null so I don't make BSML angry.
            // Never fetch it from SESL again because then I have 2 different instances of a settings object that aren't in sync.
            this.activeEnvironmentSettings = specificEnvironmentSettingsLoader?.ActivelyLoadedSettings;
        }

        public void HandlePreviewerDidFinishEvent(bool state)
        {
            // Get it from the MSTH every time the previewer is refreshed
            this.activeEnvironmentSettings = transitionHelper?.activelyLoadedSettings;

            this.gameObject.SetActive(state);
            if (groupListSettingComponent != null && state)
            {
                groupChoices = Enumerable.Range(0, transitionHelper.environmentLightGroups.Count).Cast<object>().ToList();
                groupId = "0";
                groupListSettingComponent.values = groupChoices;
                groupListSettingComponent.UpdateChoices();
                groupListSettingComponent.dropdown.SelectCellWithIdx(0);

                parser.EmitEvent("refresh-view");
            }
        }
    }
}
