using HarmonyLib;

namespace BetterStaticLights
{
    [HarmonyPatch(typeof(DefaultEnvironmentEventsFactory), nameof(DefaultEnvironmentEventsFactory.InsertDefaultEnvironmentEvents), MethodType.Normal)]
    internal class DefaultEnvironmentEffectsPatch
    {
        internal static PluginConfig Config = Plugin.Instance.Config;

        internal static bool Prefix(ref BeatmapData beatmapData, ref DefaultEnvironmentEvents defaultEnvironmentEvents, ref EnvironmentLightGroups environmentLightGroups)
        {
            if (defaultEnvironmentEvents == null || defaultEnvironmentEvents.isEmpty)
            {
                foreach (LightSetV2 lightSet in Config.lightSets)
                {
                    if (lightSet.Enabled) 
                        beatmapData.InsertBeatmapEventData(new BasicBeatmapEventData(0, lightSet.EventType, lightSet.UseSecondaryColor ? 1 : 5, 1));
                }

                return false;
            }

            else return true;
        }
    }
}
