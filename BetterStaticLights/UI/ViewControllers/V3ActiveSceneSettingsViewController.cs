using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.Configuration;
using SiraUtil.Logging;
using System.Collections.Generic;
using System.Linq;
using Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3scene.bsml")]
    [HotReload(RelativePathToLayout = @"../BSML/v3scene.bsml")]
    public class V3ActiveSceneSettingsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly SiraLog logger;
        [Inject] private readonly SpecificEnvironmentSettingsLoader specificEnvironmentSettingsLoader;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly TimeTweeningManager tweeningManager;

        private PreviewerConfigurationData previewerConfigurationData;
        private SpecificEnvironmentSettingsLoader.SpecificEnvironmentSettings activeEnvironmentSettings = null!;

        #region BSML
        [UIParams] private readonly BSMLParserParams parser;

        [UIObject("loading-parent")]
        public GameObject loadParent;

        [UIObject("settings-parent")]
        public GameObject settingsParent;

        [UIComponent("environment-list-setting")]
        public DropDownListSetting envListSetting;

        [UIComponent("apply-scene-button")]
        public Button applySceneButton;

        [UIValue("v3-environment-list")]
        public List<object> v3Environments => MockSceneTransitionHelper.v3Environments;

        [UIValue("env-setting")]
        public string environmentSetting
        {
            get => MockSceneTransitionHelper.GetNormalizedSceneName(previewerConfigurationData.environmentKey);
            set => previewerConfigurationData.environmentKey = value;
        }

        [UIComponent("light-group-list-component")]
        public DropDownListSetting groupListSettingComponent;

        [UIValue("light-group-value")]
        public string groupId
        {
            get => _groupId.ToString();
            set
            {
                groupId = value;
                _groupId = int.Parse(value);
            }
        }
        private int _groupId = 0;

        [UIValue("light-group-choices")]
        public List<object> lightGroupChoices = new List<object>() { "0" };

        [UIComponent("group-color-component")]
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
        private void UpdateViewForGroup(int idx)
        {
            SpecificEnvironmentSettingsLoader.LightGroupSettings groupSettings = activeEnvironmentSettings.LightGroupSettings[idx];
            if (groupSettings != null)
            {
                this._groupId = groupSettings.GroupId;

                Color groupColor = groupSettings.GroupColor;
                Color finalColor = groupColor.ColorWithAlpha(groupSettings.Brightness);
                tweeningManager.RestartTween(new ColorTween(Color.red, finalColor, delegate (Color color)
                {
                    transitionHelper.SetColorForGroup(transitionHelper.environmentLightGroups[idx], color);
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
                activeEnvironmentSettings.LightGroupSettings[_groupId].Brightness = value;

                LightGroup groupAtIdx = transitionHelper.environmentLightGroups[_groupId];
                if (groupAtIdx != null)
                    transitionHelper.SetColorForGroup(groupAtIdx, activeEnvironmentSettings.LightGroupSettings[_groupId].GroupColor.ColorWithAlpha(value));

                base.NotifyPropertyChanged();
            }
        }

        [UIAction("handle-list-did-change")]
        private void EnvironmentDidChangeEvent(string value)
        {
            applySceneButton.interactable = !string.Equals(previewerConfigurationData.environmentKey, MockSceneTransitionHelper.GetSerializableSceneName(value));
        }

        [UIAction("save-and-apply-env-setting")]
        private void ApplyScene()
        {
            environmentSetting = MockSceneTransitionHelper.GetSerializableSceneName(envListSetting.Value.ToString());
            applySceneButton.interactable = false;

            transitionHelper.RefreshPreviewer(true, previewerConfigurationData.environmentKey);
        }

        #endregion

        [Inject]
        internal void Construct(PluginConfig config)
        {
            this.previewerConfigurationData = config.PreviewerConfigurationData;
            this.activeEnvironmentSettings = specificEnvironmentSettingsLoader.ActivelyLoadedSettings;
            transitionHelper.previewerDidFinishEvent += HandlePreviewerDidFinishEvent;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            transitionHelper.RefreshPreviewer(true, previewerConfigurationData.environmentKey);
        }

        private void HandlePreviewerDidFinishEvent(bool state)
        {
            loadParent.SetActive(!state);
            settingsParent.SetActive(state);

            if (groupListSettingComponent != null && state)
            {
                _groupId = 0;
                this.activeEnvironmentSettings = transitionHelper.activelyLoadedSettings;
                lightGroupChoices = Enumerable.Range(0, transitionHelper.environmentLightGroups.Count).Cast<object>().ToList();
                lightGroupChoices.RemoveAll(match => {
                    logger.Info(match);
                    return transitionHelper.ignoredLightGroups.Contains(int.Parse(match.ToString()));
                });

                groupListSettingComponent.values = lightGroupChoices;
                groupListSettingComponent.UpdateChoices();
                groupListSettingComponent.dropdown.SelectCellWithIdx(0);

                parser.EmitEvent("refresh-view");
            }
        }
    }
}
