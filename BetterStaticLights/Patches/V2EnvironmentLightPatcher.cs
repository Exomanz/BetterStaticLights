using BetterStaticLights.Configuration;
using HarmonyLib;

namespace BetterStaticLights.Patches
{
    [HarmonyPatch(typeof(DefaultEnvironmentEventsFactory), nameof(DefaultEnvironmentEventsFactory.InsertDefaultEnvironmentEvents), MethodType.Normal)]
    internal class V2EnvironmentLightPatcher
    {
        private static PluginConfig Config = Plugin.Instance.Config;

        internal static bool Prefix(ref BeatmapData beatmapData, ref DefaultEnvironmentEvents defaultEnvironmentEvents)
        {
            if (defaultEnvironmentEvents == null || defaultEnvironmentEvents.isEmpty)
            {
                foreach (LightSetV2 lightSet in Config.lightSets)
                {
                    if (lightSet.enabled)
                        beatmapData.InsertBeatmapEventData(new BasicBeatmapEventData(0, lightSet.eventType, lightSet.useSecondaryColor ? 1 : 5, 1));
                }

                return false;
            }

            else return true;
        }
    }
}
