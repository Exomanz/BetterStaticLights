namespace BetterStaticLights.Configuration
{
    internal class PreviewerConfigurationData
    {
        public string environmentKey = "WeaveEnvironment";
        public bool isFirstTimePreviewingEver = true;

        public PreviewerConfigurationData() { }

        internal PreviewerConfigurationData(string environmentKey, bool isFirstTimePreviewingEver)
        {
            this.environmentKey = environmentKey;
            this.isFirstTimePreviewingEver = isFirstTimePreviewingEver;
        }
    }
}
