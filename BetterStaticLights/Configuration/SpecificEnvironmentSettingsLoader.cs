using IPA.Config.Stores.Converters;
using IPA.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace BetterStaticLights.Configuration
{
    public class SpecificEnvironmentSettingsLoader
    {
        public partial class SpecificEnvironmentSettings
        {
            private string _environmentName;
            private int _configId;
            private int _lightGroupsCount;
            private int _directionalLightsCount;
            private bool _hasGradientBackground;
            private List<LightGroupSettings> _lightGroupSettings;
            private List<DirectionalLightSettings> _directionalLightSettings;

            public string EnvironmentName => _environmentName;
            public int ConfigId => _configId;
            public int LightGroupsCount => _lightGroupsCount;
            public int DirectionalLightsCount => _directionalLightsCount;
            public bool HasGradientBackground => _hasGradientBackground;
            public List<LightGroupSettings> LightGroupSettings => _lightGroupSettings;
            public List<DirectionalLightSettings> DirectionalLightSettings => _directionalLightSettings;

            public SpecificEnvironmentSettings(string environmentName, int configId, int lightGroupsCount) : 
                this(environmentName, configId, lightGroupsCount, 0, false) 
            { }

            public SpecificEnvironmentSettings(string environmentName, int configId, int lightGroupsCount, int directionalLightsCount, bool hasGradientBackground) :
                this(environmentName, configId, lightGroupsCount, new List<LightGroupSettings>(), directionalLightsCount, new List<DirectionalLightSettings>(), hasGradientBackground)
            { }

            [JsonConstructor]
            internal SpecificEnvironmentSettings(string environmentName, int configId, int lightGroupsCount, List<LightGroupSettings> lightGroupSettings, int directionalLightsCount, List<DirectionalLightSettings> directionalLightSettings, bool hasGradientBackground)
            {
                _environmentName = environmentName;
                _configId = configId;
                _lightGroupsCount = lightGroupsCount;
                _lightGroupSettings = lightGroupSettings;
                _directionalLightsCount = directionalLightsCount;
                _directionalLightSettings = directionalLightSettings;
                _hasGradientBackground = hasGradientBackground;

                for (int i = 0; i < lightGroupsCount; i++)
                {
                    _lightGroupSettings.Add(new LightGroupSettings(i, new Color(1, 1, 1)));
                }
            }
        }

        public partial class LightGroupSettings
        {
            private int _groupId;
            private bool _enabled;
            private float _brightness;
            private Color _groupColor;

            public int GroupId => _groupId;
            public bool Enabled => _enabled;
            public float Brightness => _brightness;
            public Color GroupColor => _groupColor;

            public LightGroupSettings(int groupId, Color groupColor) :
                this(groupId, true, 1f, groupColor)
            { }

            [JsonConstructor]
            internal LightGroupSettings(int groupId, bool enabled, float brightness, Color groupColor)
            {
                _groupId = groupId;
                _enabled = enabled;
                _brightness = brightness;
                _groupColor = groupColor;
            }
        }

        public partial class DirectionalLightSettings
        {
            private bool _enabled;
            private Color _lightColor;

            public bool Enabled => _enabled;
            public Color LightColor => _lightColor;

            public DirectionalLightSettings() :
                this(true, Color.white)
            { }

            [JsonConstructor]
            internal DirectionalLightSettings(bool enabled, Color lightColor)
            {
                _enabled = enabled;
                _lightColor = lightColor;
            }
        }

        private static class LoaderConstants
        {
            public static string ConfigurationPath => Path.Combine(UnityGame.UserDataPath, "BetterStaticLights");
            public static string[] FileNames => new string[7] { "WeaveEnvironment.json", "PyroEnvironment.json", "EDMEnvironment.json", "TheSecondEnvironment.json", "LizzoEnvironment.json", "TheWeekndEnvironment.json", "RockMixtapeEnvironment.json" };
        }

        private IPALogger logger => Plugin.Instance.Logger;
        private JsonSerializerSettings serializerSettings;
        private SpecificEnvironmentSettings activelyLoadedSettings;

        public SpecificEnvironmentSettingsLoader()
        {
            serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
            };

            if (!this.ValidateDirectoryIntegrity(out bool filesInDirectory))
            {
                if (!filesInDirectory)
                {
                    this.InitializeDirectory();
                }
            }
        }

        private bool ValidateDirectoryIntegrity(out bool filesInDirectory)
        {
            filesInDirectory = true;

            if (Directory.Exists(LoaderConstants.ConfigurationPath))
            {
                int countFilesInDirectory = Directory.GetFiles(LoaderConstants.ConfigurationPath).Length;
                if (countFilesInDirectory < LoaderConstants.FileNames.Length)
                {
                    filesInDirectory = false;
                    return false;
                }

                return true;
            }

            else
            {
                Directory.CreateDirectory(LoaderConstants.ConfigurationPath);
                filesInDirectory = false;
                return false;
            }
        }

        private async void InitializeDirectory()
        {
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("WeaveEnvironment", 0, 16));
            activelyLoadedSettings = await this.LoadEnvironmentSettings("WeaveEnvironment");
        }

        public async Task<SpecificEnvironmentSettings> LoadEnvironmentSettings(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string pathToReadFrom = Path.Combine(LoaderConstants.ConfigurationPath, fileName + ".json");
            try
            {
                using StreamReader streamReader = new StreamReader(pathToReadFrom, Encoding.UTF8);
                string jsonContent = await streamReader.ReadToEndAsync();
                SpecificEnvironmentSettings settingsObject = JsonConvert.DeserializeObject<SpecificEnvironmentSettings>(jsonContent, serializerSettings);

                return settingsObject;
            }
            catch (Exception ex)
            {
                logger.Error($"Error occured while loading file {fileName}.json!\n{ex}");
            }

            return default;
        }

        public async Task SaveEnvironmentSettings(SpecificEnvironmentSettings environmentSettings)
        {
            if (environmentSettings == null)
            {
                throw new ArgumentNullException(nameof(environmentSettings));
            }

            string pathToWriteTo = Path.Combine(LoaderConstants.ConfigurationPath, environmentSettings.EnvironmentName + ".json");
            try
            {
                using StreamWriter streamWriter = new StreamWriter(pathToWriteTo, false);
                string jsonContent = JsonConvert.SerializeObject(environmentSettings, Formatting.Indented, new UnityColorConverter(typeof(SpecificEnvironmentSettings)));
                await streamWriter.WriteAsync(jsonContent);
                return;
            }
            catch (Exception ex)
            {
                logger.Error($"Error occured while writing file {environmentSettings.EnvironmentName}.json!\n{ex}");
            }
        }
    }
}
