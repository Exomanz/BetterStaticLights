using BetterStaticLights.Configuration;
using SiraUtil.Affinity;
using SiraUtil.Logging;
using Zenject;

namespace BetterStaticLights.Patches
{
    internal class V2EnvironmentPatcher : IAffinity
    {
        [Inject] private readonly SiraLog logger;
        [Inject] private readonly PluginConfig config;

        [AffinityPatch(typeof(DefaultEnvironmentEventsFactory), nameof(DefaultEnvironmentEventsFactory.InsertDefaultEnvironmentEvents), AffinityMethodType.Normal)]
        [AffinityPrefix]
        public bool Prefix(ref BeatmapData beatmapData, ref BeatmapEventDataBoxGroupLists beatmapEventDataBoxGroupLists, ref DefaultEnvironmentEvents defaultEnvironmentEvents, ref EnvironmentLightGroups environmentLightGroups)
        {
            if (defaultEnvironmentEvents == null || defaultEnvironmentEvents.isEmpty)
            {
                foreach (LightSetV2 v2 in config.lightSets)
                {
                    if (v2.enabled)
                        beatmapData.InsertBeatmapEventData(new BasicBeatmapEventData(0, v2.eventType, v2.useSecondaryColor ? 1 : 5, 1));
                }

                return false;
            }

            return true;
        }
    }
}
