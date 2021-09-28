namespace BetterStaticLights
{
    public class Config
    {
        /// <summary>
        /// Simple class that has information that is used by the <see cref="HarmonyLib.HarmonyPatch"/>
        /// </summary>
        public class LightSet
        {
            /// <summary>
            /// Returns a <see cref="BeatmapEventType"/> based on the name of the <see cref="LightSet"/>
            /// </summary>
            public BeatmapEventType Type
            {
                get
                {
                    switch (Name)
                    {
                        case "BackTop":
                            return BeatmapEventType.Event0;
                        case "RingLights":
                            return BeatmapEventType.Event1;
                        case "LeftLasers":
                            return BeatmapEventType.Event2;
                        case "RightLasers":
                            return BeatmapEventType.Event3;
                        case "BottomBackSide":
                            return BeatmapEventType.Event4;
                    }

                    return Type;
                }
            }
            /// <summary>
            /// The name of the <see cref="LightSet"/>. 
            /// </summary>
            public string Name;
            /// <summary>
            /// Specifies whether the <see cref="LightSet"/> is enabled.
            /// </summary>
            public bool Enabled;
            /// <summary>
            /// Dictates whether the <see cref="LightSet"/> uses the secondary environment color.
            /// </summary>
            public bool UseSecondaryColor;

            public LightSet() { }

            /// <summary>
            /// Creates a new <see cref="LightSet"/> with the specified <paramref name="name"/>, <paramref name="enabled"/> state, and <paramref name="useSecondaryColor"/> state.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="useSecondaryColor"></param>
            public LightSet(string name, bool enabled, bool useSecondaryColor)
            {
                Name = name;
                Enabled = enabled;
                UseSecondaryColor = useSecondaryColor;
            }
        }

        public virtual LightSet BackTop { get; set; } = new LightSet("BackTop", true, false);
        public virtual LightSet RingLights { get; set; } = new LightSet("RingLights", false, false);
        public virtual LightSet LeftLasers { get; set; } = new LightSet("LeftLasers", false, false);
        public virtual LightSet RightLasers { get; set; } = new LightSet("RightLasers", false, false);
        public virtual LightSet BottomBackSide { get; set; } = new LightSet("BottomBackSide", true, true);
    }
}
