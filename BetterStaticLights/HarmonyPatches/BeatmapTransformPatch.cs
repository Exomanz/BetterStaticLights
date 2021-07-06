using HarmonyLib;
using System.Collections.Generic;

namespace BetterStaticLights.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataNoEnvironmentEffectsTransform))]
    [HarmonyPatch(nameof(BeatmapDataNoEnvironmentEffectsTransform.CreateTransformedData))]
    internal static class BeatmapTransformPatch
    {
        private static Config Config => Plugin.XConfig;

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            codes[5].opcode = Config.opCode1;
            codes[11].opcode = Config.opCode2;

            return instructions;
        }
    }
}