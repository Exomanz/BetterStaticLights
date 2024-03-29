﻿using IPA.Config.Stores.Attributes;
using IPA.Config.Stores;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BetterStaticLights
{
    internal class PluginConfig
    {
        [Ignore] internal List<LightSetV2> lightSets = new List<LightSetV2>();

        public virtual LightSetV2 BackTop { get; set; } = new LightSetV2(BasicBeatmapEventType.Event0, false);
        public virtual LightSetV2 RingLights { get; set; } = new LightSetV2(BasicBeatmapEventType.Event1, false);
        public virtual LightSetV2 LeftLasers { get; set; } = new LightSetV2(BasicBeatmapEventType.Event2, false);
        public virtual LightSetV2 RightLasers { get; set; } = new LightSetV2(BasicBeatmapEventType.Event3, false);
        public virtual LightSetV2 BottomBackSide { get; set; } = new LightSetV2(BasicBeatmapEventType.Event4, false);
    }
}
