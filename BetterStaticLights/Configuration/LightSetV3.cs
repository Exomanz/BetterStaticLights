namespace BetterStaticLights.Configuration
{
    /// <summary>
    /// Helper class which allows for easy tracking and population of new V3 Environment effects.
    /// </summary>
    internal class LightSetV3
    {
        public int groupID { get; internal set; } = 0;
        public bool enabled { get; internal set; } = false;
        public int rotX { get; internal set; } = 0;
        public int rotY { get; internal set; } = 0;
        public float brightness { get; internal set; } = 1f;

        public LightSetV3() { }

        internal LightSetV3(int groupID, bool enabled, int rotX = 0, int rotY = 0, float brightness = 1f)
        {
            this.enabled = enabled;
            this.groupID = groupID;
            this.rotX = rotX;
            this.rotY = rotY;
            this.brightness = brightness;
        }
    }
}
