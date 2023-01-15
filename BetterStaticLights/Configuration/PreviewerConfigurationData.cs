namespace BetterStaticLights.Configuration
{
    internal class PreviewerConfigurationData
    {
        public string environmentKey = "WeaveEnvironment";
        public string colorSchemeKey = "User0";
        public bool isFirstTimePreviewingEver = true;

        public PreviewerConfigurationData() { }

        internal PreviewerConfigurationData(string environmentKey, string selectedColorSchemeKey, bool isFirstTimePreviewingEver)
        {
            this.environmentKey = environmentKey;
            this.colorSchemeKey = selectedColorSchemeKey;
            this.isFirstTimePreviewingEver = isFirstTimePreviewingEver;
        }
    }
}
