using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace BetterStaticLights.Utils
{
    internal class CustomSceneTransitionSetupDataSO : ScenesTransitionSetupDataSO
    {
        [SerializeField]
        protected SceneInfo environmentSceneInfo;

        public virtual void Init(ColorSchemesSettings colorSettings, string environmentName = "Weave")
        {
        }
    }
}
