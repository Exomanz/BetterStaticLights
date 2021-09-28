using HarmonyLib;

namespace BetterStaticLights.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataNoEnvironmentEffectsTransform), MethodType.Normal)]
    [HarmonyPatch("CreateTransformedData")]
    internal static class NoEffectsTransformPatch
    {
        private static Config Config => Plugin.Config;

        [HarmonyPrefix]
        private static bool Prefix(IReadonlyBeatmapData beatmapData, ref IReadonlyBeatmapData __result)
        {
            BeatmapData copyWithoutEvents = beatmapData.GetCopyWithoutEvents();

            if (Config.BackTop.Enabled)
                copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(
                    0, Config.BackTop.Type, Config.BackTop.UseSecondaryColor ? 1 : 5, 1));

            if (Config.RingLights.Enabled)
                copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(
                    0, Config.RingLights.Type, Config.RingLights.UseSecondaryColor ? 1 : 5, 1));

            if (Config.LeftLasers.Enabled)
                copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(
                    0, Config.LeftLasers.Type, Config.LeftLasers.UseSecondaryColor ? 1 : 5, 1));

            if (Config.RightLasers.Enabled)
                copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(
                    0, Config.RightLasers.Type, Config.RightLasers.UseSecondaryColor ? 1 : 5, 1));

            if (Config.BottomBackSide.Enabled)
                copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(
                    0, Config.BottomBackSide.Type, Config.BottomBackSide.UseSecondaryColor ? 1 : 5, 1));

            foreach (BeatmapEventData beatmapEventsDatum in beatmapData.beatmapEventsData)
            {
                if (BeatmapEventTypeExtensions.IsRotationEvent(beatmapEventsDatum.type) ||
                    BeatmapEventTypeExtensions.IsSpecialEvent(beatmapEventsDatum.type))
                {
                    copyWithoutEvents.AddBeatmapEventData(beatmapEventsDatum);
                }
            }
            __result = copyWithoutEvents;

            return false;
        }
    }
}