namespace BetterStaticLights
{
    public class Config
    {
        // Kinda messy at the moment, but this is the easiest way that I found /shrug
        public virtual bool BackTop { get; set; } = true;
        public virtual bool BTSecondaryColor { get; set; } = false;

        public virtual bool RingLights { get; set; } = false;
        public virtual bool RLSecondaryColor { get; set; } = false;

        public virtual bool LeftLasers { get; set; } = false;
        public virtual bool LLSecondaryColor { get; set; } = false;

        public virtual bool RightLasers { get; set; } = false;
        public virtual bool RLSSecondaryColor { get; set; } = false;

        public virtual bool BottomBackSide { get; set; } = true;
        public virtual bool BBSSecondaryColor { get; set; } = false;
    }
}
