using System.Collections.Generic;

namespace BetterStaticLights.Configuration
{
    internal class EnvironmentLightMappings
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
    }
}
