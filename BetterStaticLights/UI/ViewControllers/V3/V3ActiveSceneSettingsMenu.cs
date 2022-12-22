using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using BeatSaberMarkupLanguage.Components.Settings;
using SiraUtil.Logging;

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

        [UIValue("v3-environment-list")]
        public List<object> v3Environments => MockSceneTransitionHelper.v3Environments;

        [UIValue("env-setting")]
        public string environmentSetting
        {
            get => MockSceneTransitionHelper.GetNormalizedSceneName(config.environmentPreview);
            set => config.environmentPreview = value;
        }

        [UIAction("#post-parse")]
        private void __Post()
        {
            if (envListSetting != null)
            {
                parser.EmitEvent("refresh-env-setting");
            }
        }

        [UIAction("save-and-apply-env-setting")]
        private void Apply()
        {
            environmentSetting = MockSceneTransitionHelper.GetSerializableSceneName(envListSetting.Value.ToString());

            loadParent.SetActive(true);
            settingsParent.SetActive(false);
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
