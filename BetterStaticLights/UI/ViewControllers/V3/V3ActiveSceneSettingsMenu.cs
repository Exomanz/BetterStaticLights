using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
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

        [UIObject("loading-parent")]
        public GameObject loadParent;

        [UIObject("settings-parent")]
        public GameObject settingsParent;

        [UIComponent("environment-list-setting")]
        public ListSetting envListSetting;

        [UIComponent("apply-button")]
        public Button applyButton;

        [UIValue("v3-environment-list")]
        public List<object> v3Environments => MockSceneTransitionHelper.v3Environments;

        [UIValue("env-setting")]
        public string environmentSetting
        {
            get => MockSceneTransitionHelper.GetNormalizedSceneName(config.environmentPreview);
            set => config.environmentPreview = value;
        }

        [UIAction("handle-list-did-change")]
        private void ListDidChangeEvent(string value)
        {
            if (!string.Equals(transitionHelper.previouslyLoadedEnvironment, MockSceneTransitionHelper.GetSerializableSceneName(value)))
            {
                applyButton.interactable = true;
                return;
            }

            applyButton.interactable = false;
        }

        [UIAction("save-and-apply-env-setting")]
        private void Apply()
        {
            environmentSetting = MockSceneTransitionHelper.GetSerializableSceneName(envListSetting.Value.ToString());
            loadParent.SetActive(true);
            settingsParent.SetActive(false);
            applyButton.interactable = false;
            base.StartCoroutine(this.ToggleActiveSettingsViewOrRefresh());
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
