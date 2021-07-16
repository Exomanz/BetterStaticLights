using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BetterStaticLights.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataNoEnvironmentEffectsTransform))]
    [HarmonyPatch(nameof(BeatmapDataNoEnvironmentEffectsTransform.CreateTransformedData))]
    internal static class BeatmapTransformPatch
    {
        private static Config Config => Plugin.XConfig;

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            codes[5].opcode = ILGenerator.eventTypeOne;
            codes[6].opcode = ILGenerator.colorTypeOne;

            codes[11].opcode = ILGenerator.eventTypeTwo;
            codes[12].opcode = ILGenerator.colorTypeTwo;

            return null;
        }
    }
}