using IPA.Config.Stores.Attributes;
using SiraUtil.Converters;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BetterStaticLights
{
    public class Config
    {
        public static OpCode opCode1;
        public static OpCode opCode2;

        [NonNullable, UseConverter(typeof(VersionConverter))]
        public virtual SemVer.Version Version { get; set; } = new SemVer.Version("0.0.0");

        public virtual string Choice1 { get; set; } = "BackTop";
        public virtual string Choice2 { get; set; } = "BottomBackSide";

        internal static List<object> LightChoices => new List<object>(5)
        {
            "BackTop",
            "RingLights",
            "LeftLasers",
            "RightLasers",
            "BottomBackSide",
            "Off"
        };
    }
}
