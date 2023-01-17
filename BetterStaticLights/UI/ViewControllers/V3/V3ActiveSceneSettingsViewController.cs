using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.Configuration;
using Polyglot;
using SiraUtil.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;

        private PreviewerConfigurationData previewerConfigurationData;
        private ColorSchemesSettings colorSchemesSettings;
        private List<object> serializedColorSchemeIds = new List<object>();
        private Dictionary<string, string> localizedToSerializedColorSchemeIds = new Dictionary<string, string>();

        #region BSML
        [UIParams] private readonly BSMLParserParams parser;

        [UIObject("loading-parent")]
        public GameObject loadParent;

        [UIObject("settings-parent")]
        public GameObject settingsParent;

        [UIComponent("environment-list-setting")]
        public DropDownListSetting envListSetting;

        [UIComponent("colorscheme-list-setting")]
        public DropDownListSetting colorSchemeListSetting;

        [UIComponent("apply-scene-button")]
        public Button applySceneButton;

        [UIComponent("apply-color-scheme-button")]
        public Button applyColorSchemeButton;

        [UIValue("v3-environment-list")]
        public List<object> v3Environments => MockSceneTransitionHelper.v3Environments;

        [UIValue("color-schemes")]
        public List<object> colorSchemesList = new List<object>();

        [UIValue("env-setting")]
        public string environmentSetting
        {
            get => MockSceneTransitionHelper.GetNormalizedSceneName(previewerConfigurationData.environmentKey);
            set => previewerConfigurationData.environmentKey = value;
        }

        [UIValue("colorscheme-setting")]
        public string colorSchemeSetting
        {
            get => previewerConfigurationData.colorSchemeKey;
            set => previewerConfigurationData.colorSchemeKey = value;
        }

#pragma warning disable IDE0051 // Events called by BSML
        [UIAction("handle-list-did-change")]
        private void EnvironmentDidChangeEvent(string value)
        {
            applySceneButton.interactable = !string.Equals(previewerConfigurationData.environmentKey, MockSceneTransitionHelper.GetSerializableSceneName(value));
        }

        [UIAction("handle-color-scheme-did-change")]
        private void ColorSchemeDidChangeEvent(string value)
        {
            applyColorSchemeButton.interactable = !string.Equals(previewerConfigurationData.colorSchemeKey, localizedToSerializedColorSchemeIds[value]);
        }

        [UIAction("save-and-apply-env-setting")]
        private void ApplyScene()
        {
            environmentSetting = MockSceneTransitionHelper.GetSerializableSceneName(envListSetting.Value.ToString());
            applySceneButton.interactable = false;

            this.SetPreviewer();
        }

        [UIAction("save-and-apply-color-scheme")]
        private void ApplyColorScheme()
        {
            colorSchemeSetting = localizedToSerializedColorSchemeIds[colorSchemeListSetting.Value.ToString()];
            applyColorSchemeButton.interactable = false;
            colorSchemesSettings.selectedColorSchemeId = colorSchemeSetting;

            this.SetPreviewer();
        }

#pragma warning restore IDE0051
        #endregion

        [Inject] 
        internal void Construct(PluginConfig config, PlayerDataModel dataModel)
        {
            this.previewerConfigurationData = config.PreviewerConfigurationData;
            this.colorSchemesSettings = dataModel.playerData.colorSchemesSettings;
            colorSchemesList.Clear();

            for (int i = 0; i < colorSchemesSettings.GetNumberOfColorSchemes(); i++)
            {
                ColorScheme schemeAtIdx = colorSchemesSettings.GetColorSchemeForIdx(i);
                serializedColorSchemeIds.Add(schemeAtIdx.colorSchemeId);

                if (schemeAtIdx.useNonLocalizedName)
                {
                    colorSchemesList.Add(schemeAtIdx.nonLocalizedName);
                    localizedToSerializedColorSchemeIds.Add(schemeAtIdx.nonLocalizedName, schemeAtIdx.colorSchemeId);
                }
                else
                {
                    string localizedName = Localization.Get(schemeAtIdx.colorSchemeNameLocalizationKey);
                    colorSchemesList.Add(localizedName);
                    localizedToSerializedColorSchemeIds.Add(localizedName, schemeAtIdx.colorSchemeId);
                }
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            transitionHelper.previewerDidFinishEvent -= ToggleParentObjects;
            transitionHelper.previewerDidFinishEvent += ToggleParentObjects;

            this.SetPreviewer();
        }

        private void ToggleParentObjects(bool state)
        {
            loadParent.SetActive(!state);
            settingsParent.SetActive(state);
        }

        private void SetPreviewer()
        {
            transitionHelper?.Update(true, previewerConfigurationData.environmentKey);
        }
    }
}
