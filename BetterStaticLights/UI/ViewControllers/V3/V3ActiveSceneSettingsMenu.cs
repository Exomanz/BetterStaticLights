using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System.Collections;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3scene.bsml")]
    [HotReload(RelativePathToLayout = @"../../BSML/v3scene.bsml")]
    public class V3ActiveSceneSettingsMenu : BSMLAutomaticViewController
    {
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly PluginConfig config;

        private bool _coroutineDone = false;

        [UIValue("is-coroutine-done")]
        public bool isCoroutineDone
        {
            get => !_coroutineDone;
            set
            {
                _coroutineDone = value;
                NotifyPropertyChanged();
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            base.StartCoroutine(this.iHateCoroutines());
        }

        private IEnumerator iHateCoroutines()
        {
            yield return base.StartCoroutine(transitionHelper?.EnvironmentPreviewRoutine(true, config.nextPreviewEnvironment));
            isCoroutineDone = true;
        }
    }
}
