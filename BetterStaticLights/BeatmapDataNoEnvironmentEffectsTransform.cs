namespace BetterStaticLights
{
    internal abstract class BeatmapDataNoEnvironmentEffectsTransform
    {
        public static IReadonlyBeatmapData CreateTransformedData(IReadonlyBeatmapData beatmapData, PluginConfig config)
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

            foreach (LightSet lightSet in config.lightSets)
            {
                if (lightSet.Enabled)
                {
                    copyWithoutEvents.InsertBeatmapEventData(new BasicBeatmapEventData(0, lightSet.EventType, lightSet.UseSecondaryColor ? 1 : 5, 1));
                }
            }

            return copyWithoutEvents;
        }
    }
}
