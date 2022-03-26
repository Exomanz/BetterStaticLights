using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BetterStaticLights
{
    public class LightSet
    {
        public bool Enabled { get; internal set; }
        public bool UseSecondaryColor { get; internal set; }

        [UseConverter(typeof(EnumConverter<BasicBeatmapEventType>))]  // when the serialization is sus!
        public readonly BasicBeatmapEventType EventType; 

        public LightSet() { }

        public LightSet(BasicBeatmapEventType type, bool useSecondaryColor)
        {
            Enabled = true;
            EventType = type;
            UseSecondaryColor = useSecondaryColor;
        }
    }
}
