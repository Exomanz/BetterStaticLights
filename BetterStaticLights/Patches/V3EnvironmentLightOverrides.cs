using SiraUtil.Logging;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BetterStaticLights.Patches
{
    internal class V3EnvironmentLightOverrides : MonoBehaviour
    {
        [SerializeField]
        private EnvironmentInfoSO environmentInfoSO;

        internal SiraLog logger;

        [Inject] private void Construct(EnvironmentInfoSO infoSO, SiraLog logger)
        {
            this.environmentInfoSO = infoSO;
            this.logger = logger;

            if (infoSO.lightGroups.lightGroupSOList.Count != 0)
            {
                this.logger.Info("Environment is a V3 Environment");
                this.logger.Info($"Initiating overrides for environment '{infoSO.environmentName}'");
            }
        }
    }
}
