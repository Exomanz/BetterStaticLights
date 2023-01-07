namespace BetterStaticLights.Configuration
{
    internal class PreviewerConfigurationData
    {
        public string selectedEnvironmentPreview = "WeaveEnvironment";
        public string colorSchemeKey = "User0";
        public bool isFirstTimePreviewingEver = true;

        public PreviewerConfigurationData() { }

        internal PreviewerConfigurationData(string selectedEnvironmentPreview, string selectedColorSchemeKey, bool isFirstTimePreviewingEver)
        {
            this.selectedEnvironmentPreview = selectedEnvironmentPreview;
            this.colorSchemeKey = selectedColorSchemeKey;
            this.isFirstTimePreviewingEver = isFirstTimePreviewingEver;
        }
    }
}
