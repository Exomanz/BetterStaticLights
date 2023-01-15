using IPA.Utilities;
using Newtonsoft.Json;
using SiraUtil.Logging;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BetterStaticLights.Configuration
{
    public class SpecificEnvironmentSettingsLoader
    {
        public partial class SpecificEnvironmentSettings
        {
            private string _environmentName;
            private int _lightGroupsCount;
            private int _directionalLightsCount;
            private bool _hasGradientBackground;
            private List<LightGroupSettings> _lightGroupSettings;
            private List<DirectionalLightSettings> _directionalLightSettings;
            private GradientLightSettings _gradientLightSettings;

            public string EnvironmentName => _environmentName;
            public int LightGroupsCount => _lightGroupsCount;
            public int DirectionalLightsCount => _directionalLightsCount;
            public bool HasGradientBackground => _hasGradientBackground;
            public List<LightGroupSettings> LightGroupSettings => _lightGroupSettings;
            public List<DirectionalLightSettings> DirectionalLightSettings => _directionalLightSettings;
            public GradientLightSettings GradientLightSettings => _gradientLightSettings;

            public SpecificEnvironmentSettings(string environmentName, int lightGroupsCount) : 
                this(environmentName, lightGroupsCount, 0, false) 
            { }

            public SpecificEnvironmentSettings(string environmentName, int lightGroupsCount, int directionalLightsCount, bool hasGradientBackground) :
                this(environmentName, lightGroupsCount, new List<LightGroupSettings>(), directionalLightsCount, new List<DirectionalLightSettings>(), hasGradientBackground, new GradientLightSettings())
            { }

            [JsonConstructor]
            internal SpecificEnvironmentSettings(string environmentName, int lightGroupsCount, List<LightGroupSettings> lightGroupSettings, int directionalLightsCount, List<DirectionalLightSettings> directionalLightSettings, bool hasGradientBackground, GradientLightSettings gradientLightSettings)
            {
                _environmentName = environmentName;
                _lightGroupsCount = lightGroupsCount;
                _lightGroupSettings = lightGroupSettings;
                _directionalLightsCount = directionalLightsCount;
                _directionalLightSettings = directionalLightSettings;
                _hasGradientBackground = hasGradientBackground;
                _gradientLightSettings = hasGradientBackground ? gradientLightSettings : null!;

                this.CreateDataStructures(lightGroupsCount, directionalLightsCount, true, gradientLightSettings);
            }

            private void CreateDataStructures(int lightGroupsCount, int directionalLightsCount, bool hasGradientBackground, GradientLightSettings gradientLightSettings)
            {
                // Create and populate LightGroupSettings
                for (int i = 0; i < lightGroupsCount; i++)
                {
                    _lightGroupSettings.Add(new LightGroupSettings(i, new Color(0, 0, 0)));
                }

                // Create and populate DirectionalLightSettings
                for (int i = 0; i < directionalLightsCount; i++)
                {
                    _directionalLightSettings.Add(new DirectionalLightSettings(i));
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
            [JsonConverter(typeof(UnityColorConverter))] public Color GroupColor => _groupColor;

            public LightGroupSettings(int groupId, Color groupColor) :
                this(groupId, true, 1f, groupColor)
            { }

            [JsonConstructor]
            internal LightGroupSettings(int groupId, bool enabled, float brightness, Color groupColor)
            {
                _groupId = groupId;
                _enabled = enabled;
                _groupColor = groupColor;

                if (brightness > 1)
                {
                    brightness /= brightness;
                    _brightness = brightness;
                }
            }
        }

        public partial class DirectionalLightSettings
        {
            private bool _enabled;
            private int _lightId;
            private Color _lightColor;

            public bool Enabled => _enabled;
            public int LightId => _lightId;
            [JsonConverter(typeof(UnityColorConverter))] public Color LightColor => _lightColor;

            public DirectionalLightSettings() :
                this(true, 0, Color.black)
            { }

            public DirectionalLightSettings(int groupId) :
                this(true, groupId, Color.black)
            { }

            [JsonConstructor]
            internal DirectionalLightSettings(bool enabled, int lightId, Color lightColor)
            {
                _enabled = enabled;
                _lightId = lightId;
                _lightColor = lightColor;
            }
        }

        public partial class GradientLightSettings
        {
            private bool _enabled;
            private Color _lightColor;

            public bool Enabled => _enabled;
            [JsonConverter(typeof(UnityColorConverter))] public Color LightColor => _lightColor;

            public GradientLightSettings() :
                this(true, Color.black)
            { }

            [JsonConstructor]
            internal GradientLightSettings(bool enabled, Color lightColor)
            {
                _enabled = enabled;
                _lightColor = lightColor;
            }
        }

        private static class LoaderConstants
        {
            public static string ConfigurationPath => Path.Combine(UnityGame.UserDataPath, "BetterStaticLights");
            public static string[] FileNames => new string[7] { "WeaveEnvironment", "PyroEnvironment", "EDMEnvironment", "TheSecondEnvironment", "LizzoEnvironment", "TheWeekndEnvironment", "RockMixtapeEnvironment" };
        }

        private SiraLog logger;
        private PluginConfig config;
        private JsonSerializerSettings serializerSettings;
        private SpecificEnvironmentSettings activelyLoadedSettings;

        internal SpecificEnvironmentSettingsLoader(SiraLog logger, PluginConfig config)
        {
            this.logger = logger;
            this.config = config;

            serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
            };

            if (!this.ValidateDirectoryIntegrity(out bool filesInDirectory))
            {
                logger.Info(filesInDirectory);

                if (!filesInDirectory)
                    this.InitializeDirectory();

                return;

            }

            this.InitLoader();
        }

        public async void InitLoader()
        {
            this.activelyLoadedSettings = await this.LoadEnvironmentSettings(config.PreviewerConfigurationData.environmentKey);
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
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("WeaveEnvironment", 16));
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("PyroEnvironment", 14, 2, true));
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("EDMEnvironment", 18));
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("TheSecondEnvironment", 14, 5, true));
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("LizzoEnvironment", 20, 4, true));
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("TheWeekndEnvironment", 35, 4, true));
            await this.SaveEnvironmentSettings(new SpecificEnvironmentSettings("RockMixtapeEnvironment", 38, 4, true));
            activelyLoadedSettings = await this.LoadEnvironmentSettings(config.PreviewerConfigurationData.environmentKey);
        }

        public async Task<SpecificEnvironmentSettings> LoadEnvironmentSettings(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!LoaderConstants.FileNames.Any(name => name.Equals(fileName)))
            {
                throw new ArgumentException("Associated settings file does not exist!", nameof(fileName));
            }

            fileName += ".json";
            string pathToReadFrom = Path.Combine(LoaderConstants.ConfigurationPath, fileName);
            try
            {
                using StreamReader streamReader = new StreamReader(pathToReadFrom, Encoding.UTF8);
                string jsonContent = await streamReader.ReadToEndAsync();
                SpecificEnvironmentSettings settingsObject = JsonConvert.DeserializeObject<SpecificEnvironmentSettings>(jsonContent, serializerSettings);

                logger.Info("Successful read from " + fileName);

                return settingsObject;
            }
            catch (Exception ex)
            {
                logger.Error($"Error occured while loading file {fileName}!\n{ex}");
            }

            return default;
        }

        public async Task SaveEnvironmentSettings(SpecificEnvironmentSettings environmentSettings)
        {
            if (environmentSettings == null)
            {
                throw new ArgumentNullException(nameof(environmentSettings));
            }

            string fileName = environmentSettings.EnvironmentName + ".json";
            string pathToWriteTo = Path.Combine(LoaderConstants.ConfigurationPath, fileName);
            try
            {
                using StreamWriter streamWriter = new StreamWriter(pathToWriteTo, false);
                string jsonContent = JsonConvert.SerializeObject(environmentSettings, serializerSettings);
                await streamWriter.WriteAsync(jsonContent);

                logger.Info("Successful write to " + fileName);

                return;
            }
            catch (Exception ex)
            {
                logger.Error($"Error occured while writing file {fileName}!\n{ex}");
            }
        }
    }
}
