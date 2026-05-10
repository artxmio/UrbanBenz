using System.IO;
using System.Text.Json;

namespace Mercedes.Services;

public class SettingsService : ISettingsService
{
    private static readonly SettingsService _instance = new();
    public static SettingsService Instance => _instance;

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Mercedes",
        "settings.json");

    public void SaveRememberedUser(string email)
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var settings = new { RememberedEmail = email };
            var json = JsonSerializer.Serialize(settings);
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // Ignore errors
        }
    }

    public string? GetRememberedEmail()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<JsonElement>(json);
                if (settings.TryGetProperty("RememberedEmail", out var email))
                    return email.GetString();
            }
        }
        catch
        {
            // Ignore errors
        }
        return null;
    }

    public void ClearRememberedUser()
    {
        try
        {
            if (File.Exists(SettingsPath))
                File.Delete(SettingsPath);
        }
        catch
        {
            // Ignore errors
        }
    }
}