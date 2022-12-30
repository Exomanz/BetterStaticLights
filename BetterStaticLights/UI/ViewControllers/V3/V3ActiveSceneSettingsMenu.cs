using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using Polyglot;
using SiraUtil.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3scene.bsml")]
    [HotReload(RelativePathToLayout = @"../../BSML/v3scene.bsml")]
    public class V3ActiveSceneSettingsMenu : BSMLAutomaticViewController
    {
        [Inject] private readonly SiraLog logger;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly PluginConfig config;
        [UIParams] private readonly BSMLParserParams parser;

        private ColorSchemesSettings colorSchemesSettings;

        [UIObject("loading-parent")]
        public GameObject loadParent;

        [UIObject("settings-parent")]
        public GameObject settingsParent;

        [UIComponent("environment-list-setting")]
        public ListSetting envListSetting;

        [UIComponent("colorscheme-list-setting")]
        public ListSetting colorSchemeListSetting;

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
            get => MockSceneTransitionHelper.GetNormalizedSceneName(config.environmentPreview);
            set => config.environmentPreview = value;
        }

        [UIValue("colorscheme-setting")]
        public string colorSchemeSetting
        {
            get => config.colorSchemeSetting;
            set => config.colorSchemeSetting = value;
        }

        [UIAction("handle-list-did-change")]
        private void EnvironmentDidChangeEvent(string value)
        {
            applySceneButton.interactable = !string.Equals(transitionHelper.previouslyLoadedEnvironment, MockSceneTransitionHelper.GetSerializableSceneName(value));
        }

        [UIAction("handle-color-scheme-did-change")]
        private void ColorSchemeDidChangeEvent(string value)
        {
        }

        [UIAction("save-and-apply-env-setting")]
        private void ApplyScene()
        {
            environmentSetting = MockSceneTransitionHelper.GetSerializableSceneName(envListSetting.Value.ToString());

            loadParent.SetActive(true);
            settingsParent.SetActive(false);
            applySceneButton.interactable = false;

            base.StartCoroutine(this.ToggleActiveSettingsViewOrRefresh());
        }

        [UIAction("save-and-apply-color-scheme")]
        private void ApplyColorScheme()
        {
        }

        [Inject] 
        internal void Construct(PlayerDataModel dataModel)
        {
            if (dataModel != null)
            {
                this.colorSchemesSettings = dataModel.playerData.colorSchemesSettings;
                if (colorSchemesList.Count == 0)
                {
                    for (int i = 0; i < colorSchemesSettings.GetNumberOfColorSchemes(); i++)
                    {
                        ColorScheme schemeAtIdx = colorSchemesSettings.GetColorSchemeForIdx(i);
                        if (schemeAtIdx.useNonLocalizedName)
                            colorSchemesList.Add(schemeAtIdx.nonLocalizedName);
                        else
                            colorSchemesList.Add(Localization.Get(colorSchemesSettings.GetColorSchemeForIdx(i).colorSchemeNameLocalizationKey));
                    }
                }
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            base.StartCoroutine(this.ToggleActiveSettingsViewOrRefresh()); // Need blocking code
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);

            loadParent.SetActive(true);
            settingsParent.SetActive(false);
        }

        private IEnumerator ToggleActiveSettingsViewOrRefresh()
        {
            yield return base.StartCoroutine(transitionHelper.SetOrChangeEnvironmentPreview(true, config.environmentPreview));
            loadParent.SetActive(false);
            settingsParent.SetActive(true);
        }
    }
}
