using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BetterStaticLights.Helpers
{
    /// <summary>
    /// Helper class which allows for easy population of Beatmap Events on V2 Legacy Environments
    /// </summary>
    internal class LightSetV2
    {
        public bool enabled { get; internal set; }
        public bool useSecondaryColor { get; internal set; }

        [UseConverter(typeof(EnumConverter<BasicBeatmapEventType>))]
        public readonly BasicBeatmapEventType eventType;

        public LightSetV2() { }

        internal LightSetV2(BasicBeatmapEventType eventType, bool useSecondaryColor)
        {
            this.enabled = true;
            this.eventType = eventType;
            this.useSecondaryColor = useSecondaryColor;
        }
    }
}
