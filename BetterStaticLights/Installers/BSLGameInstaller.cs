using BetterStaticLights.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;
using SiraUtil.Logging;
using BetterStaticLights.UI;

namespace BetterStaticLights.Installers
{
    public class BSLGameInstaller : Installer
    {
        [Inject] private readonly SiraLog logger;

        public override void InstallBindings()
        {
            var environmentInfo = Container.TryResolve<EnvironmentSceneSetupData>();

            if (environmentInfo != null)
            {
                EnvironmentInfoSO info = environmentInfo.environmentInfo;

                if (Plugin.Instance.DEBUG)
                {
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        logger.Info(SceneManager.GetSceneAt(i).name);
                    }
                }
                else logger.Info(info.sceneInfo.name);

                Container.Bind<V3EnvironmentLightOverrides>().FromNewComponentOn(new GameObject("LightOverrides")).AsSingle().WithArguments(info).NonLazy();
            }
        }
    }
}
