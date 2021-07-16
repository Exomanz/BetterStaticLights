namespace BetterStaticLights
{
    public class Config
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual int LightSetOne { get; set; } = (int)LightSets.BackTop;
        public virtual bool UseSecondarySaberColor_SetOne { get; set; } = false;
        public virtual int LightSetTwo { get; set; } = (int)LightSets.BottomBackSide;
        public virtual bool UseSecondarySaberColor_SetTwo { get; set; } = false;

        public enum LightSets
        {
            Off = 0,
            BackTop = 1,
            RingLights = 2,
            LeftLasers = 3,
            RightLasers = 4,
            BottomBackSide = 5,
        }
    }
}
