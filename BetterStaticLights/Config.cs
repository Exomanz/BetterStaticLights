using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;

namespace BetterStaticLights
{
    public class Config
    {
        public class LightSet
        {
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
            /// The name of the LightSet. 
            /// </summary>
            public string Name;
            /// <summary>
            /// Specifies whether the set is enabled.
            /// </summary>
            public bool Enabled;
            /// <summary>
            /// Dictates whether the set uses the secondary environment color.
            /// </summary>
            public bool UseSecondaryColor;

            public LightSet() { }

            /// <summary>
            /// Initializes a new <see cref="LightSet"/> with the specified <paramref name="name"/>, <paramref name="enabled"/> state, and <paramref name="color"/>
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
