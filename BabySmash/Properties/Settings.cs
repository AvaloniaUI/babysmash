using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BabySmash.Properties
{
    public sealed class Settings : INotifyPropertyChanged
    {
        static Settings()
        {
            Default = new Settings();
            var configRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configFolderName = Assembly.GetExecutingAssembly().GetName().Name;
            var configFolder = Path.Combine(configRoot, configFolderName);

            if (!File.Exists(ApplicationSettingsPath))
            {
                if (!Directory.Exists(configFolder))
                    Directory.CreateDirectory(configFolder);
                Default.Save();
            }

            Default.Reload();
        }

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


        private bool _transparentBackground;

        public bool TransparentBackground
        {
            get => _transparentBackground;
            set
            {
                if (_transparentBackground == value) return;
                _transparentBackground = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TransparentBackground)));
            }
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

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}