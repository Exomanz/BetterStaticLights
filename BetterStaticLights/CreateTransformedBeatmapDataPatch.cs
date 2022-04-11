using HarmonyLib;

namespace BetterStaticLights
{
    [HarmonyPatch(typeof(BeatmapDataTransformHelper), nameof(BeatmapDataTransformHelper.CreateTransformedBeatmapData), MethodType.Normal)]
    internal static class CreateTransformedBeatmapDataPatch
    {
        private static PluginConfig Config => Plugin.Instance.Config;

        [HarmonyPostfix] internal static void Postfix(ref IReadonlyBeatmapData beatmapData, ref EnvironmentEffectsFilterPreset environmentEffectsFilterPreset, ref IReadonlyBeatmapData __result)
        {
            if (environmentEffectsFilterPreset == EnvironmentEffectsFilterPreset.NoEffects)
            {
                __result = BeatmapDataNoEnvironmentEffectsTransform.CreateTransformedData(beatmapData, Config);
            }
        }
    }
}
