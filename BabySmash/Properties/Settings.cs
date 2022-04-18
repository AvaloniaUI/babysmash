using System;
using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace BabySmash.Properties
{
    public sealed class Settings
    {
        public bool ForceUppercase { get; set; } = true;

        public bool FadeAway { get; set; } = true;

        public bool BitmapEffects { get; set; }

        public int ClearAfter { get; set; } = 35;

        public bool MouseDraw { get; set; }

        public int FadeAfter { get; set; } = 20;

        public bool FacesOnShapes { get; set; } = true;

        public string Sounds { get; set; } = "Laughter";

        public string CursorType { get; set; } = "Hand";

        public string FontFamily { get; set; } = "Arial";

        public bool TransparentBackground { get; set; }

        private Settings()
        {
            if (!File.Exists(ApplicationSettingsPath))
            {
                Save();
            }

            Reload();
        }

        private static string ApplicationSettingsPath { get; } = GetSettingsPath();

        private static string GetSettingsPath()
        {
            var configRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configFolder = Assembly.GetExecutingAssembly().GetName().Name;
            var configFileName = $"{nameof(Settings)}.json";
            return Path.Combine(configRoot, configFolder, configFileName);
        }

        public static Settings Default { get; private set; } = new();

        public void Save()
        {
            File.WriteAllText(ApplicationSettingsPath, JsonConvert.SerializeObject(this));
        }

        public void Reload()
        {
            Default = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(ApplicationSettingsPath));
        }
    }
}