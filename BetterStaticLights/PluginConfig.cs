using BetterStaticLights.Configuration;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BetterStaticLights
{
    internal class PluginConfig
    {
        #region V2
        // Runtime list of LightSetV2's
        [Ignore] internal readonly List<LightSetV2> lightSets = new();

        public virtual LightSetV2 LS_BackTop { get; set; } = new LightSetV2(BasicBeatmapEventType.Event0, false);
        public virtual LightSetV2 LS_RingLights { get; set; } = new LightSetV2(BasicBeatmapEventType.Event1, false);
        public virtual LightSetV2 LS_LeftLasers { get; set; } = new LightSetV2(BasicBeatmapEventType.Event2, false);
        public virtual LightSetV2 LS_RightLasers { get; set; } = new LightSetV2(BasicBeatmapEventType.Event3, false);
        public virtual LightSetV2 LS_BottomBackSide { get; set; } = new LightSetV2(BasicBeatmapEventType.Event4, false);
        #endregion

        #region V3
        public virtual string environmentPreview { get; set; } = "WeaveEnvironment";
        public virtual bool firstTimePreviewing { get; set; } = true;

        [UseConverter(typeof(ListConverter<EnvironmentConfigurationData>))]
        public virtual List<EnvironmentConfigurationData> environmentConfigurations { get; set; } = new List<EnvironmentConfigurationData>
        {
            new EnvironmentConfigurationData(0, "WeaveEnvironment", 16),
            new EnvironmentConfigurationData(1, "PyroEnvironment", 14),
            new EnvironmentConfigurationData(2, "EDMEnvironment", 18),
            new EnvironmentConfigurationData(3, "TheSecondEnvironment", 14),
            new EnvironmentConfigurationData(4, "LizzoEnvironment", 20),
            new EnvironmentConfigurationData(5, "TheWeekndEnvironment", 35),
            new EnvironmentConfigurationData(6, "RockMixtapeEnvironment", 38)
        };
/*
        public virtual EnvironmentConfigurationData WeaveEnvironmentConfigurationData { get; set; } = new EnvironmentConfigurationData("WeaveEnvironment", 16);

        public virtual EnvironmentConfigurationData PyroEnvironmentConfigurationData { get; set; } = new EnvironmentConfigurationData("PyroEnvironment", 14);

        public virtual EnvironmentConfigurationData EDMEnvironmentConfigurationData { get; set; } = new EnvironmentConfigurationData("EDMEnvironment", 18);

        public virtual EnvironmentConfigurationData TheSecondEnvironmentConfigurationData { get; set; } = new EnvironmentConfigurationData("TheSecondEnvironment", 14);

        public virtual EnvironmentConfigurationData LizzoEnvironmentConfigurationData { get; set; } = new EnvironmentConfigurationData("LizzoEnvironment", 20);

        public virtual EnvironmentConfigurationData TheWeekndEnvironmentConfigurationData { get; set; } = new EnvironmentConfigurationData("TheWeekndEnvironment", 35);

        public virtual EnvironmentConfigurationData RockMixtapeEnvironmentConfigurationData { get; set; } = new EnvironmentConfigurationData("RockMixtapeEnvironment", 38);*/
        #endregion
    }
}
