using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterStaticLights.Helpers
{
    internal class EnvironmentLightMaps
    {
        internal class EnvironmentLightMapData
        {
            public string groupName = "";
            public int groupID = 0;
            public int rotX = 0;
            public int rotY = 0;
            public float brightness = 0;

            public EnvironmentLightMapData(string groupName, int groupID, int rotX = 0, int rotY = 0, float brightness = 1)
            {
                this.groupName = groupName;
                this.groupID = groupID;
                this.rotX = rotX;
                this.rotY = rotY;
                this.brightness = brightness;
            }
        }

        // what the fuck beat games
        public readonly List<EnvironmentLightMapData> WeaveLightMapDatas = new()
        {
            new EnvironmentLightMapData("Left Side Bottom", 0),
            new EnvironmentLightMapData("Right Side Bottom", 1),
            new EnvironmentLightMapData("Left Side Top", 2),
            new EnvironmentLightMapData("Right Side Top", 3),
            new EnvironmentLightMapData("Middle Left Side Bottom", 4),
            new EnvironmentLightMapData("Middle Right Side Bottom", 5),
            new EnvironmentLightMapData("Middle Left Side Top", 6),
            new EnvironmentLightMapData("Middle Right Side Top", 7),
            new EnvironmentLightMapData("Far Left Bottom Side", 8),
            new EnvironmentLightMapData("Far Right Bottom Side", 9),
            new EnvironmentLightMapData("Far Left Top Side", 10),
            new EnvironmentLightMapData("Far Right Top Side", 11),
            new EnvironmentLightMapData("Back Top", 12),
            new EnvironmentLightMapData("Back Bottom", 13),
            new EnvironmentLightMapData("Back Left", 14),
            new EnvironmentLightMapData("Back Right", 15),
        };
    }
}
