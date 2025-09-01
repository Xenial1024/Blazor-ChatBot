using ChatBot.Models;
using Newtonsoft.Json;
namespace ChatBot.Services
{
    public class JsonSettingsStore(ILogger<JsonSettingsStore> logger)
    {
        Settings? _settings; 
        readonly ILogger<JsonSettingsStore> _logger = logger;

        static string SettingsPath(string userId) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChatBot", "Settings", $"settings-{userId}.json");
        //%appdata%\ChatBot\Settings
        public void Serialize(Settings settings, string userId)
        {
            string directory = Path.GetDirectoryName(SettingsPath(userId));
            Directory.CreateDirectory(directory);
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            System.IO.File.WriteAllText(SettingsPath(userId), json);
            _logger.LogError("Settings file written to: {Path}", SettingsPath(userId));
        }

        public Settings Deserialize(string userId)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new ArgumentNullException(nameof(userId)); 
            try
            {
                if (File.Exists(SettingsPath(userId)))
                {
                    string json = File.ReadAllText(SettingsPath(userId));
                    return JsonConvert.DeserializeObject<Settings>(json) ?? throw new InvalidOperationException("Deserializacja zwróciła nulla.");
                }
                else
                {
                    Settings settings = new() { UserId = userId };
                    Serialize(settings, userId);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd przy odczycie ustawień: {ex.Message}");
                throw;
            }
        }
    }
}