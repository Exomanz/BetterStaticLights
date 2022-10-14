using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterStaticLights
{
    /// <summary>
    /// Helper class which allows for easy population of V3 Environment Effects
    /// </summary>
    internal class LightSetV3
    {
        public bool Enabled { get; internal set; }

        public int groupId = -1;
        public float brightness = 1.0f;
        public float rotationX = 0.0f;
        public float rotationY = 0.0f;
    }
}
