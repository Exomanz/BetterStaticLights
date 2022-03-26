using HarmonyLib;

namespace BetterStaticLights.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataNoEnvironmentEffectsTransform), MethodType.Normal)]
    [HarmonyPatch("CreateTransformedData")]
    internal class NoEffectsTransformPatch
    {
        public static PluginConfig Config => Plugin.Instance.Config;

        [HarmonyPrefix]
        private static bool Prefix(IReadonlyBeatmapData beatmapData, ref IReadonlyBeatmapData __result)
        {
            static BeatmapDataItem ProcessData(BeatmapDataItem beatmapDataItem)
            {
                if (!(beatmapDataItem is BeatmapObjectData) && !(beatmapDataItem is BPMChangeBeatmapEventData) && !(beatmapDataItem is SpawnRotationBeatmapEventData))
                {
                    return null;
                }
                return beatmapDataItem;
            }

            BeatmapData copyWithoutEvents = beatmapData.GetFilteredCopy(ProcessData);

            foreach (LightSet set in Config.lightSets)
            {
                if (set.Enabled)
                {
                    copyWithoutEvents.InsertBeatmapEventData(new BasicBeatmapEventData(0, set.EventType, set.UseSecondaryColor ? 1 : 5, 1));
                }
            }

            __result = copyWithoutEvents;

            return false;
        }
    }
}