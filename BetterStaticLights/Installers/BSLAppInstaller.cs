﻿using BetterStaticLights.Configuration;
using BetterStaticLights.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace BetterStaticLights.Installers
{
    internal class BSLAppInstaller : Installer
    {
        private readonly PluginConfig config;

        public BSLAppInstaller(PluginConfig conf)
        {
            config = conf;
        }

        public override void InstallBindings()
        {
            Container.Bind<PluginConfig>().FromInstance(this.config).AsCached();
            Container.BindInterfacesAndSelfTo<SpecificEnvironmentSettingsLoader>().AsSingle().NonLazy();
            Container.BindInterfacesTo<V2EnvironmentPatcher>().AsSingle();
        }
    }
}