using IPA.Utilities;
using Newtonsoft.Json;
using SiraUtil.Logging;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterStaticLights.Configuration
{
    public class SpecificEnvironmentSettingsLoader : IAsyncInitializable
    {
        public partial class SpecificEnvironmentSettings
        {
            private string _environmentName;
            private int _lightGroupsCount;
            private int _directionalLightsCount;
            private List<LightGroupSettings> _lightGroupSettings;
            private List<DirectionalLightSettings> _directionalLightSettings;
            private GradientLightSettings _gradientLightSettings;

            public string EnvironmentName => _environmentName;
            public int LightGroupsCount => _lightGroupsCount;
            public int DirectionalLightsCount => _directionalLightsCount;
            public List<LightGroupSettings> LightGroupSettings => _lightGroupSettings;
            public List<DirectionalLightSettings> DirectionalLightSettings => _directionalLightSettings;
            public GradientLightSettings GradientLightSettings => _gradientLightSettings;

            public SpecificEnvironmentSettings(string environmentName, int lightGroupsCount) :
                this(environmentName, lightGroupsCount, 0, new GradientLightSettings())
            { }

            public SpecificEnvironmentSettings(string environmentName, int lightGroupsCount, int directionalLightsCount, GradientLightSettings gradientLightSettings) :
                this(environmentName, lightGroupsCount, new List<LightGroupSettings>(), directionalLightsCount, new List<DirectionalLightSettings>(), gradientLightSettings)
            { }

            [JsonConstructor]
            internal SpecificEnvironmentSettings(string environmentName, int lightGroupsCount, List<LightGroupSettings> lightGroupSettings, int directionalLightsCount, List<DirectionalLightSettings> directionalLightSettings, GradientLightSettings gradientLightSettings)
            {
                _environmentName = environmentName;
                _lightGroupsCount = lightGroupsCount;
                _directionalLightsCount = directionalLightsCount;
                _lightGroupSettings = lightGroupSettings;
                _directionalLightSettings = directionalLightSettings;
                _gradientLightSettings = gradientLightSettings;

                if (lightGroupSettings.Count == 0 && directionalLightSettings.Count == 0)
                    this.FillData();
            }

            private void FillData()
            {
                for (int i = 0; i < _lightGroupsCount; i++)
                {
                    this._lightGroupSettings.Add(new LightGroupSettings(i, Color.white));
                }

                for (int i = 0; i < _directionalLightsCount; i++)
                {
                    this._directionalLightSettings.Add(new DirectionalLightSettings(true, i, Color.white));
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
                    _brightness = brightness /= brightness;
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
            public static Dictionary<string, SpecificEnvironmentSettings> SpecificEnvironmentSettingsDictionary => new Dictionary<string, SpecificEnvironmentSettings>()
            {
                { "WeaveEnvironment", new SpecificEnvironmentSettings("WeaveEnvironment", 16) },
                { "PyroEnvironment", new SpecificEnvironmentSettings("PyroEnvironment", 14, 2, new GradientLightSettings()) },
                { "EDMEnvironment", new SpecificEnvironmentSettings("EDMEnvironment", 18) },
                { "TheSecondEnvironment", new SpecificEnvironmentSettings("TheSecondEnvironment", 14, 5, new GradientLightSettings()) },
                { "LizzoEnvironment", new SpecificEnvironmentSettings("LizzoEnvironment", 20, 4, new GradientLightSettings()) },
                { "TheWeekndEnvironment", new SpecificEnvironmentSettings("TheWeekndEnvironment", 35, 4, new GradientLightSettings()) },
                { "RockMixtapeEnvironment", new SpecificEnvironmentSettings("RockMixtapeEnvironment", 38, 4, new GradientLightSettings()) },
            };
        }

        private SiraLog logger;
        private PluginConfig config;
        private JsonSerializerSettings serializerSettings;
        private SpecificEnvironmentSettings activelyLoadedSettings;

        public SpecificEnvironmentSettings ActivelyLoadedSettings => activelyLoadedSettings;

        internal SpecificEnvironmentSettingsLoader(SiraLog logger, PluginConfig config)
        {
            this.logger = logger;
            this.config = config;

            serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
            };
        }

        public async Task InitializeAsync(CancellationToken token)
        {
            await this.Verify();
            activelyLoadedSettings = await this.LoadEnvironmentSettings(config.PreviewerConfigurationData.environmentKey);
        }

        private async Task Verify()
        {
            List<string> expectedFileList = LoaderConstants.FileNames.ToList();

            // If the directory doesn't exist, do the setup and exit the method.
            if (!Directory.Exists(LoaderConstants.ConfigurationPath))
            {
                Directory.CreateDirectory(LoaderConstants.ConfigurationPath);
                for (int i = 0; i < LoaderConstants.FileNames.Length; i++)
                    await this.SaveEnvironmentSettings(LoaderConstants.SpecificEnvironmentSettingsDictionary[expectedFileList[i]]);

                return;
            }

            List<string> ioFileList = Directory.GetFiles(LoaderConstants.ConfigurationPath).ToList();
            if (ioFileList.Count < expectedFileList.Count)
            {
                List<string> currentFileList = new List<string>();

                // Trim the path
                ioFileList.ForEach(file =>
                {
                    file = file.Remove(0, LoaderConstants.ConfigurationPath.Length + 1);
                    file = file.Remove(file.IndexOf('.'), 5);
                    currentFileList.Add(file);
                });

                List<string> missingFileList = new List<string>();
                logger.Error("Configuration directory is missing files:");

                for (int i = 0; i < expectedFileList.Count; i++)
                {
                    // Find missing files
                    if (!currentFileList.Contains(expectedFileList[i]))
                    {
                        logger.Error($"- {expectedFileList[i]}");
                        missingFileList.Add(expectedFileList[i]);
                    }
                }

                // Rewrite only the missing files
                for (int i = 0; i < missingFileList.Count; i++)
                    await this.SaveEnvironmentSettings(LoaderConstants.SpecificEnvironmentSettingsDictionary[missingFileList[i]]);
            }
        }

        internal async Task<SpecificEnvironmentSettings> LoadEnvironmentSettings(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null.");
            }

            if (!LoaderConstants.FileNames.Any(name => name.Equals(fileName)))
            {
                throw new ArgumentException("Associated settings file does not exist.", nameof(fileName));
            }

            fileName += ".json";
            string pathToReadFrom = Path.Combine(LoaderConstants.ConfigurationPath, fileName);
            try
            {
                using StreamReader streamReader = new StreamReader(pathToReadFrom, Encoding.UTF8);
                string jsonContent = await streamReader.ReadToEndAsync();
                SpecificEnvironmentSettings settingsObject = JsonConvert.DeserializeObject<SpecificEnvironmentSettings>(jsonContent, serializerSettings);

                return settingsObject;
            }
            catch (Exception ex)
            {
                logger.Error($"Error occured while loading file {fileName}!\n{ex}");
            }

            return default;
        }

        internal async Task SaveEnvironmentSettings(SpecificEnvironmentSettings environmentSettings)
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

                return;
            }
            catch (Exception ex)
            {
                logger.Error($"Error occured while writing file {fileName}!\n{ex}");
            }
        }
    }
}
