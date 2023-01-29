using SiraUtil.Logging;
using Zenject;

namespace BetterStaticLights.Installers
{
    public class BSLGameInstaller : Installer
    {
        [Inject] private readonly SiraLog logger;

        public override void InstallBindings()
        {
        }
    }
}
