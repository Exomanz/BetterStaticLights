using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterStaticLights
{
    [HarmonyPatch(typeof(DefaultEnvironmentEventsFactory), nameof(DefaultEnvironmentEventsFactory.InsertDefaultEnvironmentEvents), MethodType.Normal)]
    internal class PATCHER
    {
        [HarmonyPrefix]
        public static bool Prefix(ref DefaultEnvironmentEvents defaultEnvironmentEvents, ref BeatmapEventDataBoxGroupLists beatmapEventDataBoxGroupLists, EnvironmentLightGroups environmentLightGroups)
        {
            var groupList = environmentLightGroups.lightGroupDataList;
            for (int i = 0; i < groupList.Count; i++)
            {
                BeatmapEventDataBoxGroup boxGroup = BeatmapEventDataBoxGroupFactory.CreateSingleLightBeatmapEventDataBoxGroup(0, groupList[i].numberOfElements, EnvironmentColorType.Color1, 1, 0, 0);
                beatmapEventDataBoxGroupLists.Insert(groupList[i].groupId, boxGroup);
            }

            return false;
        }
    }

    public class GAMEOBJECTNAMEGETTER : MonoBehaviour
    {
        public void Awake()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore")
            {
                var objs = Resources.FindObjectsOfTypeAll<LightGroup>();
                foreach (var x in objs)
                {
                    Plugin.Instance.Logger.Info($"GameObject: {x.gameObject.name}; LightGroup ID: {x.groupId}");
                }
            }
        }
    }
}
