using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;

namespace BetterStaticLights.Configuration
{
    internal class EnvironmentConfigurationData
    {
        public int id { get; internal set; } = 0;
        public string environmentName { get; internal set; } = "";
        public int numberOfGroups { get; internal set; } = 0;

        [UseConverter(typeof(ListConverter<LightSetV3>))]
        public List<LightSetV3> activeGroupConfigurationData { get; internal set; } = new List<LightSetV3>();

        [UseConverter(typeof(ListConverter<int>))]
        public List<int> activeLightGroups { get; internal set; } = new List<int>();

        public EnvironmentConfigurationData() { }

        /// <summary>
        /// Stores information about a given environment's light configuration.
        /// </summary>
        /// <param name="environmentName">The scene name of the environment (ex. PyroEnvironment, WeaveEnvironment)</param>
        /// <param name="numberOfGroups">The 0-based number of groups in the environment</param>
        internal EnvironmentConfigurationData(int id, string environmentName, int numberOfGroups)
        {
            this.id = id;
            this.environmentName = environmentName;
            this.numberOfGroups = numberOfGroups;
        }
    }
}
