using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BetterStaticLights.UI.FlowCoordinators;
using System;
using Zenject;

namespace BetterStaticLights.UI
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        [Inject] private readonly MainFlowCoordinator mainFlowCoordinator;
        [Inject] private readonly BSLParentFlowCoordinator parentFlowCoordinator;
        private MenuButton menuButton;

        public void Initialize()
        {
            menuButton = new MenuButton("Better Static Lights", SummonFlowCoordinator);
            MenuButtons.instance.RegisterButton(this.menuButton);
        }

        private void SummonFlowCoordinator()
        {
            mainFlowCoordinator.PresentFlowCoordinator(parentFlowCoordinator, animationDirection: HMUI.ViewController.AnimationDirection.Horizontal);
        }

        public void Dispose()
        {
            if (BSMLParser.IsSingletonAvailable && MenuButtons.IsSingletonAvailable)
                MenuButtons.instance.UnregisterButton(this.menuButton);
        }
    }
}
