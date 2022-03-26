using IPA.Config.Stores.Attributes;
using IPA.Config.Stores;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BetterStaticLights
{
    internal class PluginConfig
    {
        [Ignore] internal List<LightSet> lightSets = new List<LightSet>();

        public virtual LightSet BackTop { get; set; } = new LightSet(BasicBeatmapEventType.Event0, false);
        public virtual LightSet RingLights { get; set; } = new LightSet(BasicBeatmapEventType.Event1, false);
        public virtual LightSet LeftLasers { get; set; } = new LightSet(BasicBeatmapEventType.Event2, false);
        public virtual LightSet RightLasers { get; set; } = new LightSet(BasicBeatmapEventType.Event3, false);
        public virtual LightSet BottomBackSide { get; set; } = new LightSet(BasicBeatmapEventType.Event4, false);
    }
}
