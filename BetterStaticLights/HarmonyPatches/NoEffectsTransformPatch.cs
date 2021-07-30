using HarmonyLib;
using static BeatmapEventType;

namespace BetterStaticLights.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataNoEnvironmentEffectsTransform), MethodType.Normal)]
    [HarmonyPatch("CreateTransformedData")]
    internal static class NoEffectsTransformPatch
    {
        static Config Config => Plugin.XConfig;

        [HarmonyPrefix]
        private static bool Prefix(IReadonlyBeatmapData beatmapData, ref IReadonlyBeatmapData __result)
        {
            BeatmapData copyWithoutEvents = beatmapData.GetCopyWithoutEvents();

            if (Config.BackTop)
            {
                // Some weird base-game behaviour made me have to flip the two events
                if (Config.BTSecondaryColor) copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event0, 1));
                else copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event0, 5));
            }
            if (Config.RingLights)
            {
                if (Config.RLSecondaryColor) copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event1, 5));
                else copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event1, 1));
            }
            if (Config.LeftLasers)
            {
                if (Config.LLSecondaryColor) copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event2, 5));
                else copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event2, 1));
            }
            if (Config.RightLasers)
            {
                if (Config.RLSSecondaryColor) copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event3, 5));
                else copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event3, 1));
            }
            if (Config.BottomBackSide)
            {
                if (Config.BBSSecondaryColor) copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event4, 5));
                else copyWithoutEvents.AddBeatmapEventData(new BeatmapEventData(0f, Event4, 1));
            }

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