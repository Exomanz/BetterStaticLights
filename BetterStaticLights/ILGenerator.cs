using BetterStaticLights.HarmonyPatches;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BetterStaticLights
{
    /// <summary>
    /// Provides <see cref="OpCode"/> values for the <see cref="BeatmapTransformPatch.Transpiler(IEnumerable{CodeInstruction})"/> to use during patching.
    /// </summary>
    internal static class ILGenerator
    {
        private static MethodBase originalMethod = AccessTools.DeclaredMethod(typeof(BeatmapDataNoEnvironmentEffectsTransform), "CreateTransformedData");
        private static HarmonyMethod transpiler = new HarmonyMethod(AccessTools.DeclaredMethod(typeof(BeatmapTransformPatch), "Transpiler"));

        public static Config Config => Plugin.XConfig;
        public static OpCode eventTypeOne;
        public static OpCode eventTypeTwo;
        public static OpCode colorTypeOne;
        public static OpCode colorTypeTwo;

        public static void Generate()
        {
            if (Config.LightSetOne == Config.LightSetTwo)
                Plugin.Logger.Warn("Both choices are identical. Was this intentional?");

            eventTypeOne = opCodesList[Config.LightSetOne];
            eventTypeTwo = opCodesList[Config.LightSetTwo];

            colorTypeOne = Config.UseSecondarySaberColor_SetOne ? colorTypeOne = OpCodes.Ldc_I4_5 : colorTypeOne = OpCodes.Ldc_I4_1;
            colorTypeTwo = Config.UseSecondarySaberColor_SetTwo ? colorTypeTwo = OpCodes.Ldc_I4_5 : colorTypeTwo = OpCodes.Ldc_I4_1;

            if (Plugin.HarmonyID != null)
                Plugin.HarmonyID.Patch(originalMethod, null, null, transpiler);
        }

        private static List<OpCode> opCodesList = new List<OpCode>(5)
        {
            OpCodes.Ldc_I4_M1,
            OpCodes.Ldc_I4_0,
            OpCodes.Ldc_I4_1,
            OpCodes.Ldc_I4_2,
            OpCodes.Ldc_I4_3,
            OpCodes.Ldc_I4_4,
        };
    }
}
