using BetterStaticLights.Configuration;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
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

        internal class V3EnvironmentConfigurationData
        { 
            public string mapName { get; }
            public LightSetV3[] lightSets { get; }

            public V3EnvironmentConfigurationData() { }

            internal V3EnvironmentConfigurationData(string mapName, LightSetV3[] lightSets)
            {
                this.mapName = mapName;
                this.lightSets = lightSets;
            }
        }

        public virtual V3EnvironmentConfigurationData WeaveConfigurationData { get; set; } = new V3EnvironmentConfigurationData("Weave", new LightSetV3[16]
        {
            new LightSetV3(true, 0, brightness: 0.05f),
            new LightSetV3(true, 1, brightness: 0.1f),
            new LightSetV3(true, 2, brightness: 0.15f),
            new LightSetV3(true, 3, brightness: 0.20f),
            new LightSetV3(true, 4, brightness: 0.25f),
            new LightSetV3(true, 5, brightness: 0.30f),
            new LightSetV3(true, 6, brightness: 0.35f),
            new LightSetV3(true, 7, brightness : 0.40f),
            new LightSetV3(true, 8, brightness : 0.45f),
            new LightSetV3(true, 9, brightness : 0.50f),
            new LightSetV3(true, 10, brightness : 0.55f),
            new LightSetV3(true, 11, brightness : 0.60f),
            new LightSetV3(true, 12, brightness : 0.65f),
            new LightSetV3(true, 13, brightness : 0.70f),
            new LightSetV3(true, 14, brightness : 0.75f),
            new LightSetV3(true, 15, brightness : 0.80f),
        });
        #endregion
    }
}
