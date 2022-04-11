using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BetterStaticLights
{
    /// <summary>
    /// Helper class which allows for easy population of Beatmap Events.
    /// </summary>
    public class LightSet
    {
        public bool Enabled { get; internal set; }
        public bool UseSecondaryColor { get; internal set; }

        [UseConverter(typeof(EnumConverter<BasicBeatmapEventType>))]
        public readonly BasicBeatmapEventType EventType; 

        public LightSet() { }

        internal LightSet(BasicBeatmapEventType type, bool useSecondaryColor)
        {
            Enabled = true;
            EventType = type;
            UseSecondaryColor = useSecondaryColor;
        }
    }
}
