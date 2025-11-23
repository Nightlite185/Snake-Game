using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Windows;
namespace SnakeGame.Helpers
{
    public enum SerializeOption { Scoreboard, Settings }
    public static class SerializeHelper
    {
        #region Old remnants
        [Obsolete]
        public static string GetBindingProperty(this FrameworkElement el, DependencyProperty dp)
        {
            var binding = el.GetBindingExpression(dp);
            
            string propName = binding?.ParentBinding.Path.Path 
                ?? throw new Exception("No binding path found, make sure you provided one in xaml.");

            return propName.Split('.').Last();
        }
        #endregion
        private const string SettingsFileName = "Settings.json";
        private readonly static ImmutableDictionary<SerializeOption, string> EnumToFileName = 
        [..new Dictionary<SerializeOption, string>
        {
            [SerializeOption.Settings] = SettingsFileName,
            [SerializeOption.Scoreboard] = ScoreboardFileName,
        }];
        private const string ScoreboardFileName = "Scoreboard.json";
        private static readonly string directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SnakeGame");
        private static string GetPath(SerializeOption option) => Path.Combine(directory, EnumToFileName[option]);
        public static bool Deserialize<T>(ref T TargetObj, SerializeOption option)
        {
            Directory.CreateDirectory(directory);
            string path = GetPath(option);

            if (!File.Exists(path))
                return false;

            string json = File.ReadAllText(path);
            T? result = JsonSerializer.Deserialize<T>(json);

            if (result != null)
            {
                TargetObj = result;
                return true;
            }

            else return false;
        }
        public static void Serialize<T>(T TargetObj, SerializeOption option)
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(GetPath(option), JsonSerializer.Serialize(TargetObj));
        }
    }
}