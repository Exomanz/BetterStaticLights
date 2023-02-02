using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.Configuration;
using Polyglot;
using SiraUtil.Logging;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3scene.bsml")]
    [HotReload(RelativePathToLayout = @"../../BSML/v3scene.bsml")]
    public class V3ActiveSceneSettingsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly SiraLog logger;
        [Inject] private readonly V3LightSettingsViewController lightSettingsView;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;

        private PreviewerConfigurationData previewerConfigurationData;

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

#pragma warning disable IDE0051 // Events called by BSML
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

#pragma warning restore IDE0051
        #endregion

        [Inject]
        internal void Construct(PluginConfig config, PlayerDataModel dataModel)
        {
            this.previewerConfigurationData = config.PreviewerConfigurationData;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            transitionHelper.previewerDidFinishEvent -= ToggleParentObjects;
            transitionHelper.previewerDidFinishEvent += ToggleParentObjects;
            transitionHelper.RefreshPreviewer(true, previewerConfigurationData.environmentKey);
        }

        private void ToggleParentObjects(bool state)
        {
            loadParent.SetActive(!state);
            settingsParent.SetActive(state);
        }
    }
}
