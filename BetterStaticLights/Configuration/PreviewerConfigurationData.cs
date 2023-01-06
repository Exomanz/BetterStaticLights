namespace BetterStaticLights.Configuration
{
    internal class PreviewerConfigurationData
    {
        public string selectedEnvironmentPreview = "WeaveEnvironment";
        public string colorSchemeKey = "User0";
        public int selectedGroupId = 0;
        public bool isFirstTimePreviewingEver = true;

        public PreviewerConfigurationData() { }

        internal PreviewerConfigurationData(string selectedEnvironmentPreview, string selectedColorSchemeKey, int selectedGroupId, bool isFirstTimePreviewingEver)
        {
            this.selectedEnvironmentPreview = selectedEnvironmentPreview;
            this.colorSchemeKey = selectedColorSchemeKey;
            this.selectedGroupId = selectedGroupId;
            this.isFirstTimePreviewingEver = isFirstTimePreviewingEver;
        }
    }
}
