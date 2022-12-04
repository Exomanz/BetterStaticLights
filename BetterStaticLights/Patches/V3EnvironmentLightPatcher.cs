using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterStaticLights.Patches
{
    [HarmonyPatch(typeof(DefaultEnvironmentEventsFactory), nameof(DefaultEnvironmentEventsFactory.InsertDefaultEnvironmentEvents), MethodType.Normal)]
    internal class V3EnvironmentLightPatcher
    {
        private static PluginConfig Config => Plugin.Instance.Config;

        [HarmonyPrefix]
        public static bool Prefix(ref DefaultEnvironmentEvents defaultEnvironmentEvents, ref BeatmapEventDataBoxGroupLists beatmapEventDataBoxGroupLists, ref EnvironmentLightGroups environmentLightGroups)
        {
            for (int i = 0; i < environmentLightGroups.lightGroupSOList.Count; i++)
            {
                var boxGroup = BeatmapEventDataBoxGroupFactory.CreateSingleLightBeatmapEventDataBoxGroup(0, environmentLightGroups.lightGroupSOList[i].numberOfElements, defaultEnvironmentEvents.lightGroupEvents[0]);
                beatmapEventDataBoxGroupLists.Insert(environmentLightGroups.lightGroupSOList[i].groupId, boxGroup);
            }

            return false;
        }
    }
}
