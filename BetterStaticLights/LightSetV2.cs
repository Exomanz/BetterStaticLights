﻿using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BetterStaticLights
{
    /// <summary>
    /// Helper class which allows for easy population of V2 Beatmap Events
    /// </summary>
    public class LightSetV2
    {
        public bool Enabled { get; internal set; }
        public bool UseSecondaryColor { get; internal set; }

        [UseConverter(typeof(EnumConverter<BasicBeatmapEventType>))]
        public readonly BasicBeatmapEventType EventType; 

        public LightSetV2() { }

        internal LightSetV2(BasicBeatmapEventType type, bool useSecondaryColor)
        {
            Enabled = true;
            EventType = type;
            UseSecondaryColor = useSecondaryColor;
        }
    }
}
